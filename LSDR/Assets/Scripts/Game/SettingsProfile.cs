using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDR.Game
{
    [JsonObject]
    public class SettingsProfile
    {
        public float AffineIntensity;
        public float FOV;
        public int FramerateLimit;
        public string Name;
        public bool UseClassicShaders;
        public bool UsePixelationShader;
        public bool UseDithering;
        public bool LongDrawDistance;

        // old profiles only had an on/off limit
        [JsonProperty("LimitFramerate")]
        private bool LegacyLimitFramerate
        {
            set
            {
                if (value) FramerateLimit = DisplayOptions.ORIGINAL_FRAMERATE;
            }
        }

        public SettingsProfile() { }

        public SettingsProfile(GameSettings settings)
        {
            ApplyFrom(settings);
        }

        public void ApplyFrom(GameSettings settings)
        {
            UseClassicShaders = settings.UseClassicShaders;
            UsePixelationShader = settings.UsePixelationShader;
            UseDithering = settings.UseDithering;
            FramerateLimit = settings.FramerateLimit;
            FOV = settings.FOV;
            AffineIntensity = settings.AffineIntensity;
            LongDrawDistance = settings.LongDrawDistance;
        }

        public void ApplyTo(GameSettings settings)
        {
            settings.UseClassicShaders = UseClassicShaders;
            settings.UsePixelationShader = UsePixelationShader;
            settings.UseDithering = UseDithering;
            settings.FramerateLimit = FramerateLimit;
            settings.FOV = FOV;
            settings.AffineIntensity = AffineIntensity;
            settings.LongDrawDistance = LongDrawDistance;
        }

        public static List<SettingsProfile> CreateDefaultProfiles()
        {
            return new List<SettingsProfile>
            {
                new SettingsProfile
                {
                    Name = "Classic",
                    UseClassicShaders = true,
                    UsePixelationShader = true,
                    UseDithering = true,
                    FramerateLimit = 0,
                    FOV = 50,
                    AffineIntensity = 0.4f,
                    LongDrawDistance = false,
                },
                new SettingsProfile
                {
                    Name = "Revamped",
                    UseClassicShaders = false,
                    UsePixelationShader = false,
                    UseDithering = false,
                    FramerateLimit = 0,
                    FOV = 50,
                    AffineIntensity = 0.4f,
                    LongDrawDistance = false,
                }
            };
        }
    }
}
