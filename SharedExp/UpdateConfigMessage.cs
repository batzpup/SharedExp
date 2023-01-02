namespace SharedExp
{
    public class UpdateConfigMessage
        {
            public bool SplitExp { get; set; }
            public bool CatchUpExp { get; set; }
            public UpdateConfigMessage() { }
            public UpdateConfigMessage(bool splitExp)
            {
                SplitExp = splitExp;
            }
        }
}
