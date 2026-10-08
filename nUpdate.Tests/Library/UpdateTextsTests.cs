using nUpdate.Localization;

namespace nUpdate.Tests.Library;

public class UpdateTextsTests
{
    [Fact]
    public void UpdateTexts_IntegratedFiles_NameEveryTextAndNothingElse()
    {
        var names = typeof(UpdateTexts).GetProperties().Select(p => char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)).OrderBy(n => n, StringComparer.Ordinal).ToList();
        foreach (var culture in LocalizationProvider.IntegratedCultures)
        {
            using var stream = typeof(LocalizationProvider).Assembly.GetManifestResourceStream($"nUpdate.Localization.{culture.Name}.json")!;
            var keys = Newtonsoft.Json.Linq.JObject.Parse(new StreamReader(stream).ReadToEnd()).Properties().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
            keys.ShouldBe(names, $"keys of {culture.Name}.json");
        }
    }

    [Fact]
    public void UpdateTexts_RoundTripThroughJson()
    {
        var texts = new UpdateTexts();
        foreach (var property in typeof(UpdateTexts).GetProperties())
            property.SetValue(texts, property.Name + "!");
        var restored = Serializer.Deserialize<UpdateTexts>(Serializer.Serialize(texts))!;
        foreach (var property in typeof(UpdateTexts).GetProperties())
            property.GetValue(restored).ShouldBe(property.Name + "!");
    }
}
