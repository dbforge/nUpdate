using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UserInterfaceTest
{
    public partial class MainDialog : Form
    {
        private readonly UpdaterUI _updaterUI;

        public MainDialog()
        {
            InitializeComponent();

            var manager = new UpdateManager(new Uri("http://localhost/dyn/updates.json"), "<RSAKeyValue><Modulus>rQ7p8WZF5vafxiVfqpX+ZSxsSoAJsXk8M81d2rgO5Al5ywQmkhJh6u3rpwkiJ+1yJf8LqKj5FSjhMMOlMH0R1RRCGJIft6lGj0AuZUP5brq5uo5Yp18VqbxaAU17xeRZcatQq6hqdN0FY3t0E0dH0Vm1MBAA2WgLx9TtHCksYNaUQUbxseSHKB4tzg6Qsqfe8jk/uMmODTLlVd/8G/SJXp524cT9pJUiFrW2FxLUJPZouoOr2dcWdCvyl1cLIKVk3mQL5L4XF8DBQXZ5gkDul+CrpynoBKtgtRgxqiahyBIGV884s9Pqb4+tJ9OoEoY+4oa1lQhcsQeurFbNzyqVBtjNZt9311IuXIalG+U6PlBPCCYepASa0xKCQ+U//nGYkpEgAGFAHVPU1olYMuNbxTBImFVVqbhb45HHqZClRyxhQrYfQArWB0nVC6OjrvbVAq9VSY9sMUEeVyTJPo5nRJrnPPAsF2t7CCtEjz+GPc32ZnX3Si/wjuLVQvULMyMCXKN/ALfSxHQ1CnEkv0Ym+0IZymyaF4LZvDoO+YHj50+2shaOouvEtm+zdt7TOx7ObRpItGFkvA/b5CjXZT1vhqleN9Q+FpKBCIuuXk9CEsxTtMp5UilOP1S6p0qacp0o2HlhXtkUq+wK7A0d1w+SjcBBwVQceFcQCTWz/VmGcV9U9DXQI1tnYPSYN8iwp3d442LooWQObJdDbwOfcCFznsgR7oy0d6CLdp5oSjB8CbW7mQKvSqGhS4yQlKrBVvOOT+gpKTTjypxZ4IQjBja5/VgJQaAg1tH0wOjkV/44UU/SdUxGiEElDytIvzCaYkhqASE2n7ekeGTSR74SfhjKbsmdypFzSvCCSNfjLCG5RkkcJOiyawBwf6NT2FD3Fc4a9hMOKY8mu0JPeAuSJUEY5e0cB0peJieyaq++fE+mSAGeQOVE7MkD98IMb4P/dEruIjzSrb1Y2AJXp4PODckd5XS+OR3fcHJTP2vxyGjaUMnZFLcB5AiPcdjX2qnXGwnr8/NesOLpvIZUmrxd75io3Uo/6lX7HD3v9SLcusykzrMyVrjD5h2G0+m/yojtWwrEVNPJqU7on01/50+h0/MPBTu8K/QPL+ggV6ekVrgi8VOq2qw//fj1NXJLS0+KYmcgZM1MGs1rTGrBCRDoAySDPezCCUETv7pghrStpH+UKfPdiTf22HIfvvo6H87ht4DPmF03bfGQUqbFjMyDQjHbu2n94usllVUbrGk+fjyhtx3/BsdCu4ql7eqJExu37FFnC5+xpmRPlQWvFu/9dq37L47oFlroITJWe87vBQn6hbvVAMi7ZoptefhR/d1cdfzFZEBtIP0Ms4g4uv9QvUNNmQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));
            manager.UseDynamicUpdateUri = true;
            // manager.HttpAuthenticationCredentials = new NetworkCredential("trade", "test123");
            _updaterUI = new UpdaterUI(manager, SynchronizationContext.Current);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            _updaterUI.ShowUserInterface();
        }

        private void hiddenSearchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _updaterUI.UseHiddenSearch = true;
        }
    }
}