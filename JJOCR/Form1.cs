using PaddleOCRSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe;

namespace Ocr
{
    public partial class Form1 : Form
    {
        //PaddleOCREngine engine { get; set; }
        public int hkId = 0;
        IniFiles ini = new IniFiles();
        OCRModelConfig config;
        OCRParameter oCRParameter;
        PaddleOCREngine engine;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OcrInit();
            LoadConfig();
            Init();

            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            //SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            BaseWindow bw = new BaseWindow(this, components, notifyIcon1);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Hide();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (!HotKey.UseHotKey(m))
            {
                base.WndProc(ref m);
            }

        }
        public bool Init()
        {
            try
            {
                HotKey.RemoveHotKey(Handle, hkId);
                hkId = HotKey.AddHotKey(Handle, hotKeyControl1.FuncKey, hotKeyControl1.VKey, () =>
                {
                    OcrStart();
                });

                //string[] tempstr = textBox1.Text.Split('+');

                //uint mod = 0;
                //string[] fsModifiersStrs = tempstr[0].Split(',');
                //for (int i = 0; i < fsModifiersStrs.Length; i++)
                //{
                //    mod += (uint)Enum.Parse(typeof(HotKey.FsModifiers), fsModifiersStrs[i].ToUpper());
                //}
                //string vkStr = tempstr[1].Trim();
                //Keys vk = (Keys)Enum.Parse(typeof(Keys), vkStr);

                //hkId = HotKey.AddHotKey(Handle, mod, vk, () =>
                //{
                //    OcrStart();
                //});
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("快捷键指定错误!", "出错了!");
            }
            return false;
        }
        public void OcrInit()
        {
            config = new OCRModelConfig();
            string root = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            string modelPathroot = root + @"\inference";
            //检测模型
            config.det_infer = modelPathroot + @"\ch_PP-OCRv3_det_infer";
            //方向模型
            config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            //文字识别模型
            config.rec_infer = modelPathroot + @"\ch_PP-OCRv3_rec_infer";
            config.keys = modelPathroot + @"\ppocr_keys.txt";
            oCRParameter = new OCRParameter();
            oCRParameter.enable_mkldnn = false;
            engine = new PaddleOCREngine(config, oCRParameter);

        }


        private void OcrStart()
        {
            Th th = new Th(() =>
            {
                try
                {
                    Image image = Sc.CaptureSelectedRegion();
                    if (image != null)
                    {
                        Clock.S("识别开始");
                        OCRStructureResult ocrResult = new OCRStructureResult();
                        ocrResult = engine.DetectStructure(image);
                        List<TextBlock> textBlocks = ocrResult.TextBlocks;
                        string s = "";
                        foreach (var item in textBlocks)
                        {
                            s += item.Text + "\r\n";
                        }
                        Clipboard.SetText(s);
                        string endStr = Clock.E("用时:");
                        if (checkBox1.Checked && textBlocks[0].Text != "")
                            notifyIcon1.ShowBalloonTip(2, "识别结果 " + endStr, s, ToolTipIcon.None);
                        Console.WriteLine(s);
                        Console.WriteLine("");
                        image.Dispose();
                        //engine.Dispose();

                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                }
            });

            th.Start();
        }



        private void LoadConfig()
        {
            if (!File.Exists(ini.iniFilePath))
            {
                SaveConfig();
            }
            this.hotKeyControl1.Value = ini.Read("SET", "hotkey");
            this.checkBox1.Checked = ini.Read("SET", "show_notify") == "True";
            this.checkBox2.Checked = ini.Read("SET", "mini_run") == "True";
        }
        private bool SaveConfig()
        {
            bool res = false;
            try
            {
                ini.Write("SET", "hotkey", this.hotKeyControl1.Value);
                ini.Write("SET", "show_notify", this.checkBox1.Checked.ToString());
                ini.Write("SET", "mini_run", this.checkBox2.Checked.ToString());
                res = true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return res;

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("about.txt");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Init()) return;
            if(SaveConfig()) notifyIcon1.ShowBalloonTip(2, "配置已保存", " ", ToolTipIcon.None);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1", "12");
        }

        private void hotKeyControl1_Load(object sender, EventArgs e)
        {

        }
    }
}


//public void Test()
//{
//    OpenFileDialog ofd = new OpenFileDialog();
//    ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
//    if (ofd.ShowDialog() != DialogResult.OK) return;

//    //模型配置，使用默认值
//    StructureModelConfig structureModelConfig = null;

//    //表格识别参数配置，使用默认值
//    StructureParameter structureParameter = new StructureParameter();
//    //初始化表格识别引擎
//    PaddleStructureEngine engine = new PaddleOCRSharp.PaddleStructureEngine(null, structureParameter);
//    //表格识别，返回结果是html格式的表格形式
//    string result = engine.StructureDetectFile(ofd.FileName);

//    //添加边框线，方便查看效果
//    string css = "<style>table{ border-spacing: 0pt;} td { border: 1px solid black;}</style>";
//    result = result.Replace("<html>", "<html>" + css);

//    //保存到本地
//    string name = Path.GetFileNameWithoutExtension(ofd.FileName);
//    if (!Directory.Exists(Environment.CurrentDirectory + "\\out"))
//    { Directory.CreateDirectory(Environment.CurrentDirectory + "\\out"); }
//    string savefile = $"{Environment.CurrentDirectory}\\out\\{name}.html";
//    File.WriteAllText(savefile, result);

//    //打开网页查看效果
//    Process.Start("explorer.exe", savefile);
//}