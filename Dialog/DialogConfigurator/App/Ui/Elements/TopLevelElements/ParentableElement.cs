using System.Collections.Generic;

namespace DialogConfigurator.App.Ui.Elements.TopLevelElements;

internal abstract class ParentableElement(string id = "") : UIElement(id) {
    private readonly Queue<UIElement> _Elements = new();

    internal void Enqueue(UIElement element) => _Elements.Enqueue(element);

    internal override void Draw() {
        while (_Elements.Count > 0) {
            _Elements.Dequeue().Draw();
        }
    }
}
