using System;

namespace DialogConfigurator.App.Ui;

internal interface IInput {
    Func<string> Consume { get; set; }
}