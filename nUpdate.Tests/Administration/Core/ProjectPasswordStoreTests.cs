using Microsoft.AspNetCore.DataProtection;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Security;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ProjectPasswordStoreTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public async Task ProjectPasswordStore_RemembersPerProject()
    {
        var id = Guid.NewGuid();
        (await _context.Passwords.GetAsync(id)).ShouldBeNull();
        await _context.Passwords.SetAsync(id, "pw");
        (await _context.Passwords.GetAsync(id)).ShouldBe("pw");
        _context.FileSystem.File.ReadAllText(_context.Paths.PasswordsFile).ShouldNotContain("\"pw\"");
        await _context.Passwords.SetAsync(id, "pw2");
        (await _context.Passwords.GetAsync(id)).ShouldBe("pw2");

        var other = new ProjectPasswordStore(_context.FileSystem, _context.Paths,
            new DataProtectionCredentialProtector(new EphemeralDataProtectionProvider()));
        (await other.GetAsync(id)).ShouldBeNull();

        await _context.Passwords.RemoveAsync(id);
        await _context.Passwords.RemoveAsync(id);
        (await _context.Passwords.GetAsync(id)).ShouldBeNull();

        _context.FileSystem.File.WriteAllText(_context.Paths.PasswordsFile, "{broken");
        (await _context.Passwords.GetAsync(id)).ShouldBeNull();
        await _context.Passwords.SetAsync(id, "pw3");
        (await _context.Passwords.GetAsync(id)).ShouldBe("pw3");
        _context.FileSystem.File.WriteAllText(_context.Paths.PasswordsFile, "null");
        (await _context.Passwords.GetAsync(id)).ShouldBeNull();

        await Should.ThrowAsync<ArgumentException>(() => _context.Passwords.SetAsync(id, ""));
        Should.Throw<ArgumentNullException>(() => new ProjectPasswordStore(null!, _context.Paths, _context.Protector));
        Should.Throw<ArgumentNullException>(() =>
            new ProjectPasswordStore(_context.FileSystem, null!, _context.Protector));
        Should.Throw<ArgumentNullException>(() => new ProjectPasswordStore(_context.FileSystem, _context.Paths, null!));
    }
}
