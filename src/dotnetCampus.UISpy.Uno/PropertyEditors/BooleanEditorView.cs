namespace dotnetCampus.UISpy.Uno.PropertyEditors;

public class BooleanEditorView : UserControl
{
    public BooleanEditorView(bool initialValue)
    {
        this.DataContext(
            new BindableBooleanEditorModel().WithModel(x => x.Model.Value = initialValue),
            (v, vm) => v
                .Content(new ComboBox()
                    .Items(
                    [
                        true,
                        false,
                    ])
                    .SelectedValue(x => x.Binding(() => vm.Value).TwoWay())
                ));
    }
}

public partial record BooleanEditorModel
{
    public bool Value { get; set; }
}
