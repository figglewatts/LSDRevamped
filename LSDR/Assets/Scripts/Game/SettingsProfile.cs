using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDR.Game
{
    [JsonObject]
    public class SettingsProfile
    {
        public float AffineIntensity;
        public float FOV;
        public bool LimitFramerate;
        public string Name;
        public bool UseClassicShaders;
        public bool UsePixelationShader;
        public bool LongDrawDistance;

        public SettingsProfile() { }

        public SettingsProfile(GameSettings settings)
        {
            ApplyFrom(settings);
        }

        public void ApplyFrom(GameSettings settings)
        {
            UseClassicShaders = settings.UseClassicShaders;
            UsePixelationShader = settings.UsePixelationShader;
            LimitFramerate = settings.LimitFramerate;
            FOV = settings.FOV;
            AffineIntensity = settings.AffineIntensity;
            LongDrawDistance = settings.LongDrawDistance;
        }

        public void ApplyTo(GameSettings settings)
        {
            settings.UseClassicShaders = UseClassicShaders;
            settings.UsePixelationShader = UsePixelationShader;
            settings.LimitFramerate = LimitFramerate;
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
                    LimitFramerate = false,
                    FOV = 50,
                    AffineIntensity = 0.4f,
                    LongDrawDistance = false,
                },
                new SettingsProfile
                {
                    Name = "Revamped",
                    UseClassicShaders = false,
                    UsePixelationShader = false,
                    LimitFramerate = false,
                    FOV = 50,
                    AffineIntensity = 0.4f,
                    LongDrawDistance = true,
                }
            };
        }
    }
}
