namespace TranslateRESX.Dialog
{
    public interface IDialogView
    {
        string Title { get; set; }

        string Message { get; set; }

        string BoldMessage { get; set; }

        bool Error { get; set; }
    }
}
