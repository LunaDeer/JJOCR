using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.HotkeyControl
{
    public partial class HotKeyControl : UserControl
    {
        //public byte funcKey;
        //private Keys vKey;

        [Category("KeyGroup")]
        public byte FuncKey
        {
            get;
            set;
        }

        [Category("KeyGroup")]
        public Keys VKey
        {
            get;
            set;
        }


        [Category("KeyGroup")]
        public string Value { get => this.textBox1.Text; set { this.textBox1.Text = value; LoadStr(Value); } }

        public override string ToString()
        {
            if (VKey == Keys.None) return "";
            return $"{(GetBit(FuncKey, 0) ? "ALT|" : "")}{(GetBit(FuncKey, 1) ? "CTRL|" : "")}{(GetBit(FuncKey, 2) ? "SHIFT|" : "")}{(VKey != Keys.None ? "" + VKey.ToString() : "")}";
        }
        public void LoadStr(string text)
        {
            try
            {
                if (text == "") return;
                textBox1.Text = text;
                string[] tempstr = text.Split('|');
                FuncKey = 0;
                for (int i = 0; i < tempstr.Length - 1; i++)
                {
                    switch (tempstr[i].ToUpper())
                    {
                        case "ALT":
                            FuncKey = SetBit(FuncKey, 0, true);
                            break;
                        case "CTRL":
                            FuncKey = SetBit(FuncKey, 1, true);
                            break;
                        case "SHIFT":
                            FuncKey = SetBit(FuncKey, 2, true);
                            break;
                    }
                }
                VKey = (Keys)Enum.Parse(typeof(Keys), tempstr[tempstr.Length - 1]);
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }

        public HotKeyControl()
        {
            InitializeComponent();

            this.textBox1.KeyDown += (o, e) =>
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back)
                {
                    FuncKey = 0;
                    VKey = 0;
                }
                if (e.Alt)
                {
                    FuncKey = SetBit(FuncKey, 0, true);
                }
                if (e.Control)
                {
                    FuncKey = SetBit(FuncKey, 1, true);
                }
                if (e.Shift)
                {
                    FuncKey = SetBit(FuncKey, 2, true);
                }
                if ((e.KeyValue >= 33 && e.KeyValue <= 40) ||
                    (e.KeyValue >= 65 && e.KeyValue <= 90) ||   //a-z/A-Z
                    (e.KeyValue >= 112 && e.KeyValue <= 123) ||  //F1-F12
                    (e.KeyValue >= 48 && e.KeyValue <= 57))
                {
                    VKey = e.KeyCode;
                }
                //e.Handled = true;
                Value = ToString();
            };
            this.textBox1.KeyUp += (o, e) =>
            {
                //e.Handled = true;
            };
        }


        private void HotKeyControl_Load(object sender, EventArgs e)
        {
            textBox1.Text = ToString();
            textBox1.ReadOnly = true;
        }

        public static byte SetBit(byte b, int bitIndex, bool bitValue)
        {
            if (bitIndex < 0 || bitIndex >= 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0 到 7 之间。");
            }

            if (bitValue)
            {
                // 将指定位设为 1
                return (byte)(b | (1 << bitIndex));
            }
            else
            {
                // 将指定位设为 0
                return (byte)(b & ~(1 << bitIndex));
            }
        }

        // 读取 byte 中指定位的值
        public static bool GetBit(byte b, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex >= 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "位索引必须在 0 到 7 之间。");
            }

            // 将指定位右移至最低位，并将其与 1 进行与运算，得到指定位的值
            return ((b >> bitIndex) & 1) == 1;
        }
    }


}
