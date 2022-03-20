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
using System.Net;
using System.Net.Mail;

namespace SendMail
{
    public partial class SendMailForm : MaterialForm
    {
        string? AttachmentFile1 { get; set; }
        string? AttachmentFile2 { get; set; }
        string? AttachmentFile3 { get; set; }

        public bool Attachment1 = false;
        public bool Attachment2 = false;
        public bool Attachment3 = false;

        public SendMailForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            hideBtn.Enabled = false;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            _ = sendMail();
        }

        protected async Task sendMail()
        {
            var fromAddress = new MailAddress(senderEmailTxt.Text, senderNameTxt.Text);
            var toAddress = new MailAddress(recEmailTxt.Text, recNameTxt.Text);
            string fromPassword = senderPasswordTxt.Text;
            string subject = this.subjectTxt.Text;
            string body = bodyRichTB.Text;
            Attachment attachment1;
            Attachment attachment2;
            Attachment attachment3;
            AttachmentFile1 = "";
            AttachmentFile2 = "";
            AttachmentFile3= "";
            await Task.Run(() =>
            {
                var smtp = new SmtpClient
                {
                    Host = smtpHostTxt.Text,
                    Port = Convert.ToInt32(smtpPortTxt.Text),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                try
                {
                    smtp.EnableSsl = SSL(enableSSLBtn);
                    {
                        message.IsBodyHtml = HTML(htmlCheckBox);

                        switch (attFileCheckBox1.Checked)
                        {
                            case true:
                                {
                                    attachment1 = new Attachment(fileName: Parse(AttachmentFile1));
                                    message.Attachments.Add(attachment1);
                                    break;
                                }
                            case false:
                                break;
                            default:
                        }

                        switch (attFileCheckBox2.Checked)
                        {
                            case true:
                                {
                                    attachment2 = new Attachment(fileName: Parse(AttachmentFile2));
                                    message.Attachments.Add(attachment2);
                                    break;
                                }
                            case false:
                                break;
                            default:
                        }

                        switch (attFileCheckBox3.Checked)
                        {
                            case true:
                                {
                                    attachment3 = new Attachment(fileName: Parse(AttachmentFile3));
                                    message.Attachments.Add(attachment3);
                                    break;
                                }
                            case false:
                                break;
                            default:
                        }

                        smtp.Send(message);
                        _ = MessageBox.Show("Message Sent", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        message.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    message.Dispose();
                }
            });
        }

        protected static string Parse(string x)
        {
            return string.IsNullOrWhiteSpace(x) ? "" : x;
        }

        protected static bool SSL(RadioButton radioButton1)
        {
            return radioButton1.Checked switch
            {
                true => true,
                _ => false,
            };
        }

        protected static bool HTML(CheckBox checkBox)
        {
            return checkBox.Checked switch
            {
                true => true,
                _ => false,
            };
        }
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
            senderPasswordTxt.PasswordChar = '\0';
            hideBtn.Enabled = true;
            showBtn.Enabled = false;
        }

        private void hideBtn_Click(object sender, EventArgs e)
        {
            senderPasswordTxt.PasswordChar = '•';
            hideBtn.Enabled = false;
            showBtn.Enabled = true;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            switch (Attachment1)
            {
                case false when Attachment2 == false && Attachment3 == false:
                    AttachmentFile1 = attach.Text;
                    Attachment1 = true;
                    attFileCheckBox1.Checked = true;
                    attFileCheckBox1.Enabled = true;
                    attach.Text = "";
                    break;

                case true when Attachment2 == false && Attachment3 == false:
                    AttachmentFile2 = attach.Text;
                    Attachment2 = true;
                    attFileCheckBox2.Checked = true;
                    attFileCheckBox2.Enabled = true;
                    attach.Text = "";
                    break;

                case true when Attachment2 && Attachment3 == false:
                    AttachmentFile3 = attach.Text;
                    Attachment3 = true;
                    attFileCheckBox3.Checked = true;
                    attFileCheckBox3.Enabled = true;
                    attach.Text = "";
                    break;

                case true when Attachment2 && Attachment3 == true:
                    MessageBox.Show("Attachment is full. Click 'Clear All' button to reset attachments.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    attach.Text = "";
                    break;
                default:
                    break;
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            AttachmentFile2 = "";
            AttachmentFile2 = "";
            AttachmentFile3 = "";
            attFileCheckBox1.Checked = false;
            attFileCheckBox1.Enabled = false;
            attFileCheckBox2.Checked = false;
            attFileCheckBox2.Enabled = false;
            attFileCheckBox3.Checked = false;
            attFileCheckBox3.Enabled = false;
            Attachment1 = false;
            Attachment2 = false;
            Attachment3 = false;
        }
    }
}