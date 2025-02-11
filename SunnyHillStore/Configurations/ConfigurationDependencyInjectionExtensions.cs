namespace SunnyHillStore.Api.Configurations
{
    public static class ConfigurationDependencyInjectionExtensions
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            BindCloudinaryConfiguration(services, configuration);
        }
        public static void BindCloudinaryConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var cloudinarySettings = new CloudinarySettings();
            configuration.Bind("Cloudinary", cloudinarySettings);
            services.AddSingleton(cloudinarySettings);
        }
    }
}
