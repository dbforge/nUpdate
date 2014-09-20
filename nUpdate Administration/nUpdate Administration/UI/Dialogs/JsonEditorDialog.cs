﻿// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Drawing;
using System.IO;
using System.Text;
using FastColoredTextBoxNS;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class JsonEditorDialog : BaseDialog
    {
        private readonly Style _argumentsStyle = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        private readonly Style _commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly Style _keyWordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
/*
        private Style _stringStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
*/

        public JsonEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The content of the language file.
        /// </summary>
        public string LanguageContent { get; set; }

        /// <summary>
        ///     The name of the language/culture.
        /// </summary>
        public string CultureName { get; set; }

        private void JSONEditorDialog_Load(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.Default.GetBytes(JsonHelper.FormatJson(LanguageContent));
            codeTextBox.Text = Encoding.UTF8.GetString(bytes);
        }

        private void codeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(_argumentsStyle);
            e.ChangedRange.SetStyle(_argumentsStyle, @"\{[0-9]+\}?.");
            e.ChangedRange.SetStyle(_keyWordStyle, @"null");
            //e.ChangedRange.SetStyle(stringStyle, "(?<=(?<!\)").+?(?=(?<!\)")", RegexOptions.Global);
            e.ChangedRange.SetStyle(_commentStyle, @".*//.*$");
            e.ChangedRange.SetFoldingMarkers("{", "}");
        }

        private void saveLanguageButton_Click(object sender, EventArgs e)
        {
            try
            {
                JToken.Parse(codeTextBox.Text);
            }
            catch (FormatException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "The JSON-code is invalid.", ex, PopupButtons.Ok);
                return;
            }

            try
            {
                string filePath = Path.Combine(Program.Path, "Localization", String.Format("{0}.json", CultureName));
                using (File.Create(filePath))
                {
                }
                File.WriteAllText(filePath, codeTextBox.Text);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the language-file.", ex, PopupButtons.Ok);
            }
            finally
            {
                Close();
            }
        }
    }
}