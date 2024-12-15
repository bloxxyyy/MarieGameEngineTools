using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

args = ["/../../../FileUpdater.cs"];

if (args.Length < 1)
{
    Console.WriteLine("Usage: RefactorTool <file-path>");
    //return;
}

string currentWorkingDirectory = Directory.GetCurrentDirectory();
Console.WriteLine("Current Working Directory: " + currentWorkingDirectory);

//string fullFilePath = currentWorkingDirectory + args[0];
string fullFilePath = currentWorkingDirectory + "/../../../FileUpdater.cs";
Console.WriteLine("Full Path: " + fullFilePath);

if (!File.Exists(fullFilePath))
{
    Console.WriteLine($"Error: File not found at '{fullFilePath}'");
    return;
}

string originalCode = File.ReadAllText(fullFilePath);
AdhocWorkspace workspace = new();
SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(originalCode);
SyntaxNode root = syntaxTree.GetRoot();



SyntaxToken tempParameterName = SyntaxFactory.Identifier("skibidi");
TypeSyntax tempParameterType = SyntaxFactory.ParseTypeName("string");
ParameterSyntax newParameter = SyntaxFactory.Parameter(tempParameterName).WithType(tempParameterType);
ParameterListSyntax updatedParameterList = SyntaxFactory.ParameterList().AddParameters(newParameter);

AddNewMethod(ref root, "FileUpdater", "NewMethod", "void", updatedParameterList);
AddNewMethod(ref root, "FileUpdater", "NewMethodTwo", "void", updatedParameterList);

AddParameterForMethod(ref root, "UpdateData", "x", "int");
AddParameterForMethod(ref root, "UpdateData", "y", "int");
AddParameterForMethod(ref root, "NewMethod", "x", "int");


MethodDeclarationSyntax methodDeclarationSyntax = root.DescendantNodes()
    .OfType<MethodDeclarationSyntax>()
    .FirstOrDefault(m => m.Identifier.Text == "UpdateData")
    ?? throw new Exception("Method UpdateData not found");

foreach (SyntaxNode creation in methodDeclarationSyntax.DescendantNodes().Where(IsObjectCreation))
{
    LocalDeclarationStatementSyntax parentStatement = creation.Ancestors()
        .OfType<LocalDeclarationStatementSyntax>()
        .FirstOrDefault() ?? throw new Exception("Parent statement not found");

    Console.WriteLine(parentStatement.ToString().Trim());
}








//SyntaxNode     refactoredRoot = RefactorCode(root);
string formattedCode = Formatter.Format(root, workspace).ToFullString();

File.WriteAllText(fullFilePath, formattedCode);
Console.WriteLine($"File successfully refactored and updated at '{fullFilePath}'");


bool IsObjectCreation(SyntaxNode node)
{
    return node is ObjectCreationExpressionSyntax || node is AnonymousObjectCreationExpressionSyntax;
}

void AddNewMethod(ref SyntaxNode root, string className, string methodName, string returnType, ParameterListSyntax parameters)
{
    ClassDeclarationSyntax classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
        .FirstOrDefault(m => m.Identifier.Text == className) ?? throw new Exception($"Class {className} not found");

    TypeSyntax _returnType = SyntaxFactory.ParseTypeName(returnType);
    MethodDeclarationSyntax newMethod = SyntaxFactory.MethodDeclaration(_returnType, methodName);
    newMethod = newMethod.WithParameterList(parameters);
    BlockSyntax blockSyntax = SyntaxFactory.Block();
    newMethod = newMethod.WithBody(blockSyntax);

    ClassDeclarationSyntax updatedClass = classDeclarationSyntax.AddMembers(newMethod);
    root = root.ReplaceNode(classDeclarationSyntax, updatedClass);
}

void AddParameterForMethod(ref SyntaxNode root, string methodName, string parameterName, string parameterType)
{
    MethodDeclarationSyntax methodDeclarationSyntax = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
      .FirstOrDefault(m => m.Identifier.Text == methodName) ?? throw new Exception($"Method {methodName} not found");

    SyntaxToken _parameterName = SyntaxFactory.Identifier(parameterName);
    TypeSyntax _parameterType = SyntaxFactory.ParseTypeName(parameterType);
    ParameterSyntax newParameter = SyntaxFactory.Parameter(_parameterName).WithType(_parameterType);
    ParameterListSyntax updatedParameterList = methodDeclarationSyntax.ParameterList.AddParameters(newParameter);

    MethodDeclarationSyntax updatedMethod = methodDeclarationSyntax.WithParameterList(updatedParameterList);
    root = root.ReplaceNode(methodDeclarationSyntax, updatedMethod);
}














SyntaxNode RefactorCode(SyntaxNode root)
{
    MethodDeclarationSyntax? method = root.DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .FirstOrDefault(m => m.Identifier.Text == "UpdateData");

    if (method == null)
    {
        Console.WriteLine("No 'UpdateData' method found for refactoring.");
        return root;
    }

    IEnumerable<InvocationExpressionSyntax> fileOperations = method.DescendantNodes()
        .OfType<InvocationExpressionSyntax>()
        .Where(inv => inv.ToString().Contains("File."));

    if (fileOperations.Any())
    {
        root = ExtractMethod(root, method, fileOperations, "HandleFileOperations");

        method = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.Text == "UpdateData");

        if (method == null)
        {
            Console.WriteLine("Error: Method 'UpdateData' not found after extraction.");
            return root;
        }

        fileOperations = method.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => inv.ToString().Contains("File."));

        root = Skibidi(root, method, fileOperations, "HandleFileOperations");
    }

    return root;
}

SyntaxNode ExtractMethod(
    SyntaxNode root,
    MethodDeclarationSyntax originalMethod,
    IEnumerable<InvocationExpressionSyntax> operations,
    string newMethodName
)
{
    BlockSyntax? newMethodBody = SyntaxFactory.Block(
        operations.Select(operation => SyntaxFactory.ExpressionStatement(operation.WithoutTrivia()))
    );

    MethodDeclarationSyntax? newMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), newMethodName)
        .WithModifiers(
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)
            )
        )
        .WithParameterList(originalMethod.ParameterList)
        .WithBody(newMethodBody);


    if (originalMethod.Parent is not ClassDeclarationSyntax classDeclaration)
        throw new Exception("Class declaration is null");

    ClassDeclarationSyntax newClassDeclaration = classDeclaration.AddMembers(newMethod);
    return root.ReplaceNode(classDeclaration, newClassDeclaration);
}

SyntaxNode Skibidi(
    SyntaxNode root,
    MethodDeclarationSyntax originalMethod,
    IEnumerable<InvocationExpressionSyntax> operations,
    string newMethodName
)
{
    SeparatedSyntaxList<ArgumentSyntax> separatedList = SyntaxFactory.SeparatedList<ArgumentSyntax>();
    foreach (ParameterSyntax parameter in originalMethod.ParameterList.Parameters)
    {
        separatedList = separatedList.Add(
            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Identifier))
        );
    }

    ExpressionStatementSyntax newInvocation = SyntaxFactory.ExpressionStatement(
        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(newMethodName))
            .WithArgumentList(SyntaxFactory.ArgumentList(separatedList))
    );

    if (originalMethod.Body == null)
        throw new Exception("Method body is null");
    BlockSyntax updatedMethodBody = originalMethod.Body.ReplaceNodes(operations, (_, __) => newInvocation.Expression);

    return root.ReplaceNode(originalMethod, originalMethod.WithBody(updatedMethodBody));
}
