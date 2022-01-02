// ValidationManager.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    public class ValidationManager
    {
        public static bool Validate(Control owner)
        {
            return !owner.Controls.OfType<TextBox>().Any(c => string.IsNullOrEmpty(c.Text)) &&
                   owner.Controls.Cast<Control>().Where(x => x.HasChildren).All(Validate);
        }

        public static bool ValidateAndIgnore(Control owner, IEnumerable<TextBox> fieldsToIgnore)
        {
            return !owner.Controls.OfType<TextBox>().Where(c => !fieldsToIgnore.Contains(c))
                       .Any(c => string.IsNullOrEmpty(c.Text)) && owner.Controls.Cast<Control>()
                       .Where(x => x.HasChildren).All(x => ValidateAndIgnore(x, fieldsToIgnore));
        }
    }
}