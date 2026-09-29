using ArdCRM.Core.Interfaces;
using ArdCRM.Core.Kararlar;
using ArdCRM.Data.Adapters.Karar;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Karar motoru DI kaydı.
///
/// Davranış, ERP adapter'ıyla birebir aynıdır:
///   Jev:ApiAnahtari tanımlıysa  → JEV birincil, kural motoru yedek
///   tanımlı değilse             → yalnızca kural motoru
///
/// JEV yapılandırılmış olsa bile Jev:SemaDogrulandi = false olduğu sürece
/// adapter ağ çağrısı yapmaz; her istek kural motoruna düşer. Bu kasıtlıdır.
/// </summary>
public static class KararMotoruServiceCollectionExtensions
{
    public static IServiceCollection AddKararMotoru(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var secenekler = new JevSecenekleri();
        configuration.GetSection(JevSecenekleri.BolumAdi).Bind(secenekler);

        services.AddSingleton(secenekler);
        services.AddSingleton(new HassasVeriFiltresi());
        services.AddSingleton(VarsayilanKurallar.Olustur());
        services.AddScoped<KuralTabanliKararMotoru>();

        if (string.IsNullOrWhiteSpace(secenekler.ApiAnahtari))
        {
            services.AddScoped<IKararMotoru>(sp => sp.GetRequiredService<KuralTabanliKararMotoru>());
            return services;
        }

        services.AddHttpClient<JevKararMotoru>();

        services.AddScoped<IKararMotoru>(sp => new YedekliKararMotoru(
            birincil: sp.GetRequiredService<JevKararMotoru>(),
            yedek:    sp.GetRequiredService<KuralTabanliKararMotoru>()));

        return services;
    }
}
