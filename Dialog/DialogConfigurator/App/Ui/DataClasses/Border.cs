namespace DialogConfigurator.App.Ui.DataClasses;

internal struct Border(string colorHex, int size) {
    internal string ColorHex = colorHex;
    internal int Size = size;
}
