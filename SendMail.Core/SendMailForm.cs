#region MIT License
// Copyright (c) 2022 Glenn Alon
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using MaterialSkin;
using MaterialSkin.Controls;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;

namespace SendMail
{
    public partial class SendMailForm : MaterialForm
    {
        // These strings will display the file directory of an attachment file.
        private string? AttachmentFile1 { get; set; }
        private string? AttachmentFile2 { get; set; }
        private string? AttachmentFile3 { get; set; }

        // These booleans would represent the state of an attachment queue.
        private bool IsAttachmentEnabled1 { get; set; } = false;
        private bool IsAttachmentEnabled2 { get; set; } = false;
        private bool IsAttachmentEnabled3 { get; set; } = false;

        protected internal SendMailForm()
        {
            InitializeComponent();

            // Initializes the MaterialSkin package's theme for the Form.
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void SendMailForm_Load(object sender, EventArgs e)
        {
            hideBtn.Enabled = false;
            smtpPortTxt.ShortcutsEnabled = false;
            senderPasswordTxt.PasswordChar = '\u25CF';         

        }
   
        private async void SendMail() // This method allows the email to be sent asynchronously.
        {
            MailAddress senderAddress = new(senderEmailTxt.Text, senderNameTxt.Text);
            MailAddress recipientAddress = new(recEmailTxt.Text, recNameTxt.Text);
            await Task.Run(() =>
            {
                string senderPassword = senderPasswordTxt.Text;
                bool smtp_port_parsing = int.TryParse(smtpPortTxt.Text, out int smtp_port);
                if (!smtp_port_parsing)
                {
                    MessageBox.Show("Converting the given port to an integer have failed.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    smtp_port = int.Parse(smtpPortTxt.Text);
                }

                string host = smtpHostTxt.Text;
                if (host == null)
                {
                    MessageBox.Show("The SMTP server host is empty or null.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SmtpClient smtp = new()
                {
                    Host = smtpHostTxt.Text,
                    Port = smtp_port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderAddress.Address, senderPassword)
                };

                string subject = subjectTxt.Text;
                string body = messageBox.Text;
                using var message = new MailMessage(senderAddress, recipientAddress)
                {
                    Subject = subject,
                    Body = body
                };

                smtp.EnableSsl = enableSSLBtn.Checked; // This uses the SSL security when the radio button is checked.
                message.IsBodyHtml = htmlCheckBox.Checked; // This uses HTML for the e-mail message when checked.

                if (AttachmentFile1 != null)
                {
                    using Attachment attachment1 = new(AttachmentFile1);
                    AddAttachment(message, attFileCheckBox2, attachment1);
                }

                if (AttachmentFile2 != null)
                {
                    using Attachment attachment2 = new(AttachmentFile2);
                    AddAttachment(message, attFileCheckBox2, attachment2);
                }
                
                if (AttachmentFile3 != null)
                {
                    using Attachment attachment3 = new(AttachmentFile3);
                    AddAttachment(message, attFileCheckBox3, attachment3);
                }
                
                try
                {
                    smtp.Send(message);
                    MessageBox.Show("Message Sent", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                message.Dispose();
            });
        }

        private static void AddAttachment(MailMessage message, CheckBox attCheckBox, Attachment attachment)
        {
            // This method is used to determine whether or not the checkbox is checked and then determines whether or not the attachment should be added.
            if (attCheckBox.Checked)
            {
                message.Attachments.Add(attachment);
                return;
            }
            else
            {
                return;
            }
        }

        private void sendBtn_Click(object sender, EventArgs e) => SendMail(); // The button would call the SendMail method.

        private void browseBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                attach.Text = openFileDialog1.FileName;
            }
        }
        
        private void showBtn_Click(object sender, EventArgs e) 
        {
            // The button would disable itself ('Show') after it has been clicked while enabling the opposing function ('Hide') button.
            // Additionally, this would allow the users to view the input without any filters.
            senderPasswordTxt.PasswordChar = '\0';
            hideBtn.Enabled = true;
            showBtn.Enabled = false;
        }

        private void hideBtn_Click(object sender, EventArgs e)
        {
            // The button would disable itself ('Hide) after it has been clicked while enabling the opposing function ('Show') button.
            // In addition, the user input would be hidden beneath censored bullets.
            senderPasswordTxt.PasswordChar = '\u25CF';
            hideBtn.Enabled = false;
            showBtn.Enabled = true;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            switch (IsAttachmentEnabled1)
            {
                case false when IsAttachmentEnabled2 == false && IsAttachmentEnabled3 == false:
                    AttachmentFile1 = attach.Text;
                    IsAttachmentEnabled1 = true;
                    attFileCheckBox1.Checked = true;
                    attFileCheckBox1.Enabled = true;
                    attach.ResetText();
                    break;

                case true when IsAttachmentEnabled2 == false && IsAttachmentEnabled3 == false:
                    AttachmentFile2 = attach.Text;
                    IsAttachmentEnabled2 = true;
                    attFileCheckBox2.Checked = true;
                    attFileCheckBox2.Enabled = true;
                    attach.ResetText();
                    break;

                case true when IsAttachmentEnabled2 && IsAttachmentEnabled3 == false:
                    AttachmentFile3 = attach.Text;
                    IsAttachmentEnabled3 = true;
                    attFileCheckBox3.Checked = true;
                    attFileCheckBox3.Enabled = true;
                    attach.ResetText();
                    break;

                case true when IsAttachmentEnabled2 && IsAttachmentEnabled3 == true:
                    MessageBox.Show("The attachment queue is already full. To clear all attachments, click the 'Clear All' option.", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    attach.Text = string.Empty;
                    attach.ResetText();
                    break;
                default:
                    break;
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            AttachmentFile2 = string.Empty;
            AttachmentFile2 = string.Empty;
            AttachmentFile3 = string.Empty;
            attFileCheckBox1.Checked = false;
            attFileCheckBox1.Enabled = false;
            attFileCheckBox2.Checked = false;
            attFileCheckBox2.Enabled = false;
            attFileCheckBox3.Checked = false;
            attFileCheckBox3.Enabled = false;
            IsAttachmentEnabled1 = false;
            IsAttachmentEnabled2 = false;
            IsAttachmentEnabled3 = false;
        }

        private void smtpPortTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // The event limits the input to numerical values.
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}