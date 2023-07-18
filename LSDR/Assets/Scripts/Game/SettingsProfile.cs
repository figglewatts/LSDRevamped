using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDR.Game
{
    [JsonObject]
    public class SettingsProfile
    {
        public string Name;
        public bool UseClassicShaders;
        public bool UsePixelationShader;
        public bool LimitFramerate;
        public float FOV;
        public float AffineIntensity;

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
        }

        public void ApplyTo(GameSettings settings)
        {
            settings.UseClassicShaders = UseClassicShaders;
            settings.UsePixelationShader = UsePixelationShader;
            settings.LimitFramerate = LimitFramerate;
            settings.FOV = FOV;
            settings.AffineIntensity = AffineIntensity;
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
                    AffineIntensity = 0.4f
                },
                new SettingsProfile
                {
                    Name = "Revamped",
                    UseClassicShaders = false,
                    UsePixelationShader = false,
                    LimitFramerate = false,
                    FOV = 50,
                    AffineIntensity = 0.4f,
                },
            };
        }
    }
}
