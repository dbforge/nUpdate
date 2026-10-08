using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Integration.Support;

/// <summary>Exercises every operation of <see cref="ITransferProvider" /> on a connected provider; each provider has to pass it.</summary>
public static class TransferContract
{
    public static async Task ExerciseAsync(IntegrationContext context, ITransferProvider transfer, string prefix)
    {
        var directory = $"{prefix}-dir";
        (await transfer.DirectoryExistsAsync(directory)).ShouldBeFalse();
        await transfer.CreateDirectoryAsync($"{directory}/nested");
        (await transfer.DirectoryExistsAsync(directory)).ShouldBeTrue();
        (await transfer.DirectoryExistsAsync($"{directory}/nested")).ShouldBeTrue();

        var local = context.WriteFile($"{prefix}.txt", "hello " + prefix);
        var progress = new List<TransferProgress>();
        await transfer.UploadFileAsync(local, $"{directory}/nested/{prefix}.txt", new SyncProgress<TransferProgress>(progress));
        (await transfer.FileExistsAsync($"{directory}/nested/{prefix}.txt")).ShouldBeTrue();
        (await transfer.FileExistsAsync($"{directory}/nested/missing.txt")).ShouldBeFalse();
        progress.ShouldNotBeEmpty();

        var listing = await transfer.ListAsync(directory, recursive: false);
        listing.Single().Name.ShouldBe("nested");
        listing.Single().ItemType.ShouldBe(ServerItemType.Directory);
        var deep = await transfer.ListAsync(directory, recursive: true);
        deep.ShouldContain(i => i.Name == $"{prefix}.txt" && i.ItemType == ServerItemType.File && i.Size == ("hello " + prefix).Length);

        var downloaded = Path.Combine(context.Root, $"{prefix}-download.txt");
        await transfer.DownloadFileAsync($"{directory}/nested/{prefix}.txt", downloaded, new SyncProgress<TransferProgress>(progress));
        File.ReadAllText(downloaded).ShouldBe("hello " + prefix);

        await transfer.RenameAsync($"{directory}/nested/{prefix}.txt", $"{directory}/nested/renamed.txt");
        (await transfer.FileExistsAsync($"{directory}/nested/renamed.txt")).ShouldBeTrue();
        await transfer.DeleteFileAsync($"{directory}/nested/renamed.txt");
        await transfer.DeleteFileAsync($"{directory}/nested/renamed.txt");
        (await transfer.FileExistsAsync($"{directory}/nested/renamed.txt")).ShouldBeFalse();

        await transfer.UploadFileAsync(local, $"{directory}/nested/again.txt");
        await transfer.DeleteDirectoryAsync(directory);
        await transfer.DeleteDirectoryAsync(directory);
        (await transfer.DirectoryExistsAsync(directory)).ShouldBeFalse();
    }
}
