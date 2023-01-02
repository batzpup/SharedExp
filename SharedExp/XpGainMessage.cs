namespace SharedExp
{
    public class XpGainMessage
    {
        public int Which { get; set; }
        public int HowMuch { get; set; }
        public XpGainMessage() { }
        public XpGainMessage(int which, int howMuch)
        {
            Which = which;
            HowMuch = howMuch;
        }
    }
}
