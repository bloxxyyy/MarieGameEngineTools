using KokoSharpTranspiler;

args = ["../../../KokoScriptExampleProj"];
var transpiler = new Transpiler(args[0]);
transpiler.Setup();
transpiler.Transpile();
