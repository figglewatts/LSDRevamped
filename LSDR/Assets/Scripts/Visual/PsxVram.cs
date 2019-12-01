namespace LSDR.Visual
{
    /// <summary>
    /// Virtual PSX VRAM. A TIX file can be loaded into it to populate it.
    /// Renderers that want to use this need to set their materials to the ones in here.
    /// </summary>
    public static class PsxVram
    {
        // as it was in PSX datasheets
        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;
    }
}
