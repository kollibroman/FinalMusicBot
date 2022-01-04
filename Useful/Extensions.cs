namespace FinalMusicBot.Useful
{
    public static class Extensions
    {   
        public static bool ShouldPlayNext(this TrackEndReason trackEndReason)
        {
            return trackEndReason == TrackEndReason.Finished || trackEndReason == TrackEndReason.LoadFailed;
        }
    }
}