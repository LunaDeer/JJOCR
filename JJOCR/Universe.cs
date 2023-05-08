using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe;

namespace Universe
{
    public static class EnumHelper
    {
        public static T GetValByName<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }
        public static string GetNameByVal<T>(T value)
        {
            return Enum.GetName(typeof(T), value);
        }

        public static string ToStr(this DateTime dt)
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmssff");
        }
    }
    public class BaseWindow
    {
        Form win;
        System.ComponentModel.IContainer components;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        public BaseWindow(Form win, System.ComponentModel.IContainer components, NotifyIcon notifyIcon)
        {
            this.win = win;
            this.components = components;
            this.notifyIcon = notifyIcon;

            win.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            //win.Move += new System.EventHandler(this.StopAtEdge);
            win.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            //win.ShowInTaskbar = false;
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            notifyIcon.Text = Global.productName;


            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(components);
            this.contextMenuStrip2.SuspendLayout();
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(181, 70);
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(打开ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(退出ToolStripMenuItem_Click);


            this.contextMenuStrip2.ResumeLayout(false);
            notifyIcon.ContextMenuStrip = this.contextMenuStrip2;
            notifyIcon.Visible = true;
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (win.WindowState == FormWindowState.Minimized)
            {
                HideWin();
            }
        }

        public void ShowWin()
        {
            win.Show();
            win.WindowState = FormWindowState.Normal;
            win.Focus();
        }
        public void HideWin()
        {
            win.Hide();
        }

        public void CutShow()
        {
            if (win.Visible)
            {
                HideWin();
            }
            else
            {
                ShowWin();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWin();
        }


        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWin();
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void Close()
        {
            notifyIcon.Visible = false;
            HotKey.RemoveHotKey(win.Handle);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Environment.Exit(0);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideWin();

            //DialogResult dr = MessageBox.Show("是否退出?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            //if (dr == DialogResult.OK)   //如果单击“是”按钮
            //{
            //    e.Cancel = false;                 //关闭窗体
            //    Close();
            //}
            //else if (dr == DialogResult.Cancel)
            //{
            //    e.Cancel = true;                  //不执行操作
            //}
        }


        /// <summary>
        /// 停靠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StopAtEdge(object sender, EventArgs e)
        {
            int thresholdw = 7;
            System.Drawing.Rectangle screenInfo = Screen.PrimaryScreen.WorkingArea;

            if (Math.Abs(win.Location.X - 0) < thresholdw)
            {
                win.Location = new Point(0, win.Location.Y);
            }
            if (Math.Abs(win.Location.Y - 0) < thresholdw)
            {
                win.Location = new Point(win.Location.X, 0);
            }
            if (Math.Abs(win.Location.X + win.Size.Width - screenInfo.Width) < thresholdw)
            {
                win.Location = new Point(screenInfo.Width - win.Size.Width, win.Location.Y);
            }
            if (Math.Abs(win.Location.Y + win.Size.Height - screenInfo.Height) < thresholdw)
            {
                win.Location = new Point(win.Location.X, screenInfo.Height - win.Size.Height);
            }
        }
    }

    class Drag
    {
        private bool isDragging = false;
        private Point startPoint = new Point(0, 0); //记录鼠标按下去的坐标, new是为了拿到空间, 两个0无所谓的
        private Point offsetPoint = new Point(0, 0); //记录动了多少距离,然后给窗体Location赋值,要设置Location,必须用一个Point结构体,不能直接给Location的X,Y赋值
        private Form win;
        public Drag(Control obj, Form win)
        {
            this.win = win;
            obj.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown);
            obj.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove);
            obj.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUp);
        }
        private void MouseDown(object sender, MouseEventArgs e)
        {


            //如果按下去的按钮不是左键就return,节省运算资源
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //按下鼠标后,进入拖动状态:
            isDragging = true;
            //保存刚按下时的鼠标坐标
            startPoint.X = e.X;
            startPoint.Y = e.Y;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标移动时调用,检测到IsDragging为真时
            if (isDragging == true)
            {
                //用当前坐标减去起始坐标得到偏移量Offset
                offsetPoint.X = e.X - startPoint.X;
                offsetPoint.Y = e.Y - startPoint.Y;
                //将Offset转化为屏幕坐标赋值给Location,设置Form在屏幕中的位置,如果不作PointToScreen转换,你自己看看效果就好
                win.Location = win.PointToScreen(offsetPoint);
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            //左键抬起时,及时把拖动判定设置为false,否则,你也可以试试效果
            isDragging = false;
        }


    }

    public class Win
    {
        private IntPtr hwnd;
        private int _x, _y, _w, _h;

        public Win(string name)
        {
            try
            {
                hwnd = FuzzyFindWindow("", name);
                rect();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType()}:{e.Message}");
            }
        }

        public static IntPtr GetFocusWin()
        {
            return Win32Gui.GetForegroundWindow();
        }

        public override string ToString()
        {
            return $"{hwnd.ToInt64():X}|{ClassName}|{TitleName}";
        }

        public void Focus()
        {
            Win32Gui.SetForegroundWindow(hwnd);
        }

        public int OffsetX
        {
            get { return X + 8 + 1; }
        }

        public int OffsetY
        {
            get { return Y + 23 + 8 + 1; }
        }

        public bool IsFocus
        {
            get { return TitleName == GetWindowText(Win32Gui.GetForegroundWindow()); }
        }

        public string ClassName
        {
            get { return GetWindowClass(hwnd); }
        }

        public string TitleName
        {
            get { return GetWindowText(hwnd); }
        }

        public int X
        {
            get { rect(); return _x; }
            set { _x = value; Set(); }
        }

        public int Y
        {
            get { rect(); return _y; }
            set { _y = value; Set(); }
        }

        public int W
        {
            get { rect(); return _w; }
            set { _w = value; Set(); }
        }

        public int H
        {
            get { rect(); return _h; }
            set { _h = value; Set(); }
        }

        private void rect()
        {
            Win32Gui.RECT rect = new Win32Gui.RECT();
            Win32Gui.GetWindowRect(hwnd, ref rect);
            _x = rect.left;
            _y = rect.top;
            _w = rect.right - rect.left;
            _h = rect.bottom - rect.top;
        }

        private void Set()
        {
            Win32Gui.MoveWindow(hwnd, _x, _y, _w, _h, true);
        }

        public static string GetWindowText(IntPtr hwnd)
        {
            var lptrString = new StringBuilder(512);
            Win32Gui.GetWindowText(hwnd, lptrString, lptrString.Capacity);
            var title = lptrString.ToString().Trim();
            return title;
        }
        public static string GetWindowClass(IntPtr hwnd)
        {
            var lptrString = new StringBuilder(512);
            Win32Gui.GetClassName(hwnd, lptrString, lptrString.Capacity);
            return lptrString.ToString();
        }


        static IntPtr currentHwnd = IntPtr.Zero;
        /// <summary>
        /// 模糊查找窗口
        /// </summary>
        /// <param name="cName">窗口类名</param>
        /// <param name="tName">窗口名</param>
        /// <returns></returns>
        public static IntPtr FuzzyFindWindow(string cName, string tName)
        {
            currentHwnd = IntPtr.Zero;
            if (cName == "" && tName == "")
            {
                return currentHwnd;
            }
            Win32Gui.WNDENUMPROC a = callBackFunc;
            Win32Gui.EnumWindows(a, cName + "_-_-_-_" + tName);
            return currentHwnd;
            /*
             
                C/C++ code

                BOOL CALLBACK EnumWindowsProc(HWND hWnd, LPARAM lParam)
                {
                    HWND hListBox = (HWND)lParam;
                    ASSERT(hListBox);
                    TCHAR szWindow[256] = {0};
                    ::GetWindowText(hWnd, szWindow, ……
                 GetWindowText

                BOOL EnumWindows（WNDENUMPROC lpEnumFunc，LPARAM lParam），它的第二个参数LPARAM指定一个传递给回调函数的应用程序定义值，也就是说要根据具体情况你自己指定，并且这个参数与回调函数BOOL CALLBACK EnumWindowsProc(HWND hwnd,LPARAM lParam); 中的第二个参数相对应，你要做的就是在枚举窗口函数的回调函数中，取得窗口标题后就向CListBox窗口发送添加消息（LB_ADDSTRING），而发送消息函数SendMessage的第一个参数就是接受此消息的控件的句柄。所以，根据你的要求，EnumWindows函数的第二个参数就设定为your_listBox.GetSafeHwnd,然后在回调函数中的发送消息函数的第一个参数指定为它即可
             
             */
        }


        public static bool callBackFunc(IntPtr hwnd, string param1)
        {
            // 获取窗口类名。
            var lpString = new StringBuilder(512);
            Win32Gui.GetClassName(hwnd, lpString, lpString.Capacity);
            var className = lpString.ToString();

            var lptrString = new StringBuilder(512);
            Win32Gui.GetWindowText(hwnd, lptrString, lptrString.Capacity);
            var title = lptrString.ToString().Trim();

            string cName = param1.Split(new string[] { "_-_-_-_" }, StringSplitOptions.None)[0];
            string tName = param1.Split(new string[] { "_-_-_-_" }, StringSplitOptions.None)[1];

            //如果之穿窗口名,直接判断窗口名
            if (cName == "" && tName != "")
            {
                if (title.IndexOf(tName) != -1)
                {
                    Console.WriteLine($"0x{Convert.ToString((int)hwnd, 16)},{title},{tName},");
                    currentHwnd = hwnd;
                    return false;
                }
            }
            else if (className == cName)
            {
                //Console.WriteLine($"{Convert.ToString((int)hwnd, 16)},{className}");
                // 获取窗口标题。
                //Console.WriteLine($"{title},{param1[1]},{param1[0]}");
                if (title.IndexOf(tName) != -1)
                {
                    Console.WriteLine($"0x{Convert.ToString((int)hwnd, 16)},{title},{tName},");
                    currentHwnd = hwnd;
                    return false;
                }
            }
            return true;
        }
    }

    public class Win32Gui
    {
        public const int SW_HIDE = 0;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;

        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;

        public const int HWND_BOTTOM = 1;
        public const int HWND_NOTOPMOST = -2;
        public const int HWND_TOP = 0;
        public const int HWND_TOPMOST = -1;

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        // lParam类型要与下面的EnumWindows第二参数类型相同
        public delegate bool WNDENUMPROC(IntPtr hwnd, string lParam);
        [DllImport("user32")]
        public static extern int EnumWindows(WNDENUMPROC x, string lParam);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("User32.dll ", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int type);

        [DllImport("User32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //        SWP_ASYNCWINDOWPOS
        //0x4000
        //如果调用线程和拥有窗口的线程附加到不同的输入队列，系统会将请求发布到拥有该窗口的线程。 这可以防止调用线程阻止其执行，而其他线程处理请求。
        //SWP_DEFERERASE
        //0x2000
        //阻止生成 WM_SYNCPAINT 消息。
        //SWP_DRAWFRAME
        //0x0020
        //绘制在窗口的类说明中定义的框架() 窗口周围。
        //SWP_FRAMECHANGED
        //0x0020
        //使用 SetWindowLong 函数应用设置的新框架样式。 将 WM_NCCALCSIZE 消息发送到窗口，即使窗口的大小未更改也是如此。 如果未指定此标志，则仅当窗口的大小发生更改时， 才会发送WM_NCCALCSIZE 。
        //SWP_HIDEWINDOW
        //0x0080
        //隐藏窗口。
        //SWP_NOACTIVATE
        //0x0010
        //不激活窗口。 如果未设置此标志，则会激活窗口，并根据 hWndInsertAfter 参数) 的设置(将窗口移到最顶部或最顶层组的顶部。
        //SWP_NOCOPYBITS
        //0x0100
        //丢弃工作区的整个内容。 如果未指定此标志，则会在调整或重新定位窗口后保存并复制回工作区的有效内容。
        public const uint SWP_NOMOVE = 0x0002;
        //SWP_NOMOVE
        //0x0002
        //保留当前位置 (忽略 X 和 Y 参数) 。
        //SWP_NOOWNERZORDER
        //0x0200
        //不更改 Z 顺序中的所有者窗口位置。
        //SWP_NOREDRAW
        //0x0008
        //不重绘更改。 如果设置了此标志，则不执行任何形式的重绘。 这适用于工作区、非工作区(，包括标题栏和滚动条) ，以及由于移动窗口而发现的父窗口的任何部分。 设置此标志后，应用程序必须显式失效或重新绘制需要重绘的窗口和父窗口的任何部分。
        //SWP_NOREPOSITION
        //0x0200
        //与 SWP_NOOWNERZORDER 标志相同。
        //SWP_NOSENDCHANGING
        //0x0400
        //阻止窗口接收 WM_WINDOWPOSCHANGING 消息。
        public const uint SWP_NOSIZE = 0x0001;
        //0x0001
        //保留当前大小(忽略 cx 和 cy 参数) 。
        //SWP_NOZORDER
        //0x0004
        //保留当前 Z 顺序(忽略 hWndInsertAfter 参数) 。
        //SWP_SHOWWINDOW
        //0x0040
    }


    public static class FileTools
    {
        /*
		（1）. DirectoryInfo.GetFiles()：获取目录中（不包含子目录）的文件，返回类型为FileInfo[]，支持通配符查找；   
　　　　（2）. DirectoryInfo.GetDirectories()：获取目录（不包含子目录）的子目录，返回类型为DirectoryInfo[]，支持通配符查找；   
　　　　（3）. DirectoryInfo. GetFileSystemInfos()：获取指定目录下（不包含子目录）的文件和子目录，返回类型为FileSystemInfo[]，支持通配符查找；
		*/
        public static List<string> FindAllFile(string sSourcePath)
        {
            List<String> list = new List<string>();
            List<String> targetFileList = new List<string>();
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo NextFile in thefileInfo)  //遍历文件
            {
                list.Add(NextFile.FullName);
            }

            //遍历子文件夹
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                //list.Add(NextFolder.ToString());
                FileInfo[] fileInfo = NextFolder.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo NextFile in fileInfo)  //遍历文件
                    list.Add(NextFile.FullName);
            }
            return list;
        }
        public static List<string> FindFileAtSuffix(string sSourcePath, string suffix)
        {
            List<String> list = new List<string>();
            if (!Directory.Exists(sSourcePath))
                return list;
            string[] suffixList;
            suffixList = suffix.Split('|');
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo NextFile in thefileInfo)  //遍历文件
            {
                for (int i = 0; i < suffixList.Count(); i++)
                {
                    if (suffixList[i] == Path.GetExtension(NextFile.FullName))
                        list.Add(NextFile.FullName);
                }
            }

            //遍历子文件夹
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                //list.Add(NextFolder.ToString());
                FileInfo[] fileInfo = NextFolder.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo NextFile in fileInfo)  //遍历文件
                {
                    for (int i = 0; i < suffixList.Count(); i++)
                    {
                        if (suffixList[i] == Path.GetExtension(NextFile.FullName))
                            list.Add(NextFile.FullName);
                    }


                }
            }
            return list;
        }

        public static void FindTargetInAFile(string path, string target, out string targetFile, out string remarks)
        {
            targetFile = "";
            remarks = "";
            int line = 1;
            int index = -1;
            bool found = false;
            string sStuName = string.Empty;
            FileStream f = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader reader = new StreamReader(f);
            //StreamReader reader = new StreamReader(f, UnicodeEncoding.GetEncoding("GB2312"));
            f.Position = 0;
            while ((sStuName = reader.ReadLine()) != null)
            {
                //Console.WriteLine(sStuName);
                index = sStuName.IndexOf(target);
                if (index >= 0)
                {
                    if (found == false)
                    {
                        found = true;
                        targetFile = path;
                        //Console.WriteLine(path);
                    }
                    remarks = ("第" + line.ToString() + "行,第" + index.ToString() + "位置,该行为:") + sStuName;
                    //Console.WriteLine("第"+line.ToString()+"行,第"+index.ToString()+"位置,该行为:");
                    //Console.WriteLine(sStuName);
                }
                line++;
            }


            // for (int i = 0; i <= 20; i++)
            // {
            // Console.Write(F.ReadByte() + " ");
            // }

            f.Close();
        }

        public static string GetCurrentPath()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }
        public static string GetCurrentFilePath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        //.exe的目录
        public static string GetCurrentExePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
        //获取桌面
        public static string GetDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        public static string ReadTxt(string pathFile)
        {
            FileStream stream = null;
            StreamReader reader = null;
            string s = "";
            try
            {
                if (!System.IO.File.Exists(pathFile)) return "";
                stream = new FileStream(pathFile, FileMode.Open);//fileMode指定是读取还是写入
                reader = new StreamReader(stream, Encoding.UTF8);

                //writer.WriteLine("cd "+path.Substring(0,3));
                //writer.Write("start \"\" ");
                //writer.WriteLine(s);//写入一行，写完后会自动换行
                s = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return s;
        }


        public static void WriteTxt(string filePath, string s)
        {
            FileStream stream = null;
            StreamWriter writer = null;
            try
            {
                string dir = Path.GetDirectoryName(filePath);
                CreateDir(dir);
                stream = new FileStream(filePath, FileMode.Create);//fileMode指定是读取还是写入
                writer = new StreamWriter(stream, Encoding.UTF8);

                writer.Write(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        public static void CreateFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Append);
            StreamWriter wr = new StreamWriter(fs);
            wr.Close();
            fs.Close();
        }
        public static void CreateDir(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
        }


        public static void WriteLog(string folderPath, string fileName, string msg)
        {
            FileStream fs = null;
            StreamWriter logWriter = null;
            try
            {
                FileTools.CreateDir(folderPath);
                string filePath = string.Format(folderPath + @"{0}{1}.txt", fileName, DateTime.Now.ToString("yyyy-MM-dd"));
                fs = new FileStream(filePath, FileMode.Append);//fileMode指定是读取还是写入
                logWriter = new StreamWriter(fs, Encoding.Default);

                logWriter.WriteLine(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
            finally
            {
                if (logWriter != null)
                {
                    logWriter.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        public static void WriteLog(string msg)
        {
            msg = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:ffff] ") + msg;
            WriteLog(GetCurrentExePath() + @"log\", "Log", msg);
        }

        public static void DeleteFile(string fileFullPath)
        {
            // 1、首先判断文件或者文件路径是否存在
            //if (Directory.Exists(fileFullPath))
            //{
            // 2、根据路径字符串判断是文件还是文件夹
            try
            {
                FileAttributes attr = File.GetAttributes(fileFullPath);
                // 3、根据具体类型进行删除
                if (attr == FileAttributes.Directory)
                {
                    Directory.Delete(fileFullPath, true); // 3.1、删除文件夹
                }
                else
                {
                    File.Delete(fileFullPath);// 3.2、删除文件
                }
                File.Delete(fileFullPath);
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }

    public class UpperPcToolkit
    {
        /// <summary>
        /// FX3U结尾的求和校验"37 30 38 30 35 03"对这样的数据进行累加求和返回字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SumCalc2Str(string data)
        {
            return UpperPcToolkit.Str2Asill(UpperPcToolkit.SumCalc(data));
        }
        public static string Str2Asill(string str)
        {
            byte[] array = System.Text.Encoding.ASCII.GetBytes(str); //数组array为对应的ASCII数组
            string ASCIIstr = null;
            for (int i = 0; i < array.Length; i++)
            {
                int asciicode = (int)(array[i]);
                ASCIIstr += Convert.ToString(asciicode, 16) + " ";//字符串ASCIIstr2 为对应的ASCII字符串
            }
            return ASCIIstr.Trim();
        }
        public static string SumCalc(string data)
        {
            try
            {
                string[] datas = data.Trim().Split(' ');
                List<byte> bytedata = new List<byte>();
                int sum = 0;
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Length != 2)
                    {
                        throw new Exception("数据格式有误,16进制只能由0123456789ABCDEF组成,请输入\"01 02 03 07 0A FE FF\"这种格式的数据!");
                    }
                    bytedata.Add(byte.Parse(datas[i], System.Globalization.NumberStyles.AllowHexSpecifier));
                    //Console.WriteLine(bytedata[i].ToString());
                    sum += bytedata[i];
                }
                //Console.WriteLine(sum);
                //string a = Convert.ToString((byte)((sum & 0xff)), 16).PadLeft(2, '0').ToUpper();
                string a = Convert.ToString((byte)(sum), 16).PadLeft(2, '0').ToUpper();
                //Console.WriteLine(a);
                return a;
            }
            catch (Exception)
            {
                return "";
                throw;
            }


        }

        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <returns>高低8位</returns>
        public static string CRCCalc(string data)
        {
            string[] redata = new string[2];
            try
            {
                string[] datas = data.Trim().Split(' ');
                List<byte> bytedata = new List<byte>();
                foreach (string str in datas)
                {
                    if (str.Length != 2)
                    {
                        throw new Exception("数据格式有误,16进制只能由0123456789ABCDEF组成,请输入\"01 02 03 07 0A FE FF\"这种格式的数据!"); ;
                    }
                    bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                byte[] crcbuf = bytedata.ToArray();
                //计算并填写CRC校验码
                int crc = 0xffff;
                int len = crcbuf.Length;
                for (int n = 0; n < len; n++)
                {
                    byte i;
                    crc = crc ^ crcbuf[n];
                    for (i = 0; i < 8; i++)
                    {
                        int TT;
                        TT = crc & 1;
                        crc = crc >> 1;
                        crc = crc & 0x7fff;
                        if (TT == 1)
                        {
                            crc = crc ^ 0xa001;
                        }
                        crc = crc & 0xffff;
                    }
                }
                redata[1] = Convert.ToString((byte)((crc >> 8) & 0xff), 16).PadLeft(2, '0').ToUpper();
                redata[0] = Convert.ToString((byte)((crc & 0xff)), 16).PadLeft(2, '0').ToUpper();
            }
            catch (Exception)
            {
                throw;
            }
            return redata[0] + " " + redata[1];
            //return FormatHEX(redata[0]) + " " + FormatHEX(redata[1]);
        }
        /// <summary>
        /// "02 A3 1F"这种字符串类型转成8位数组
        /// </summary>
        /// <param name="sendStr">要转的字符串</param>
        /// <returns>8位数组</returns>
        public static byte[] Str2HexArr(string sendStr)
        {
            if (sendStr.Length == 0) return new byte[0];
            string[] sendStrArr = sendStr.Trim().Split(' ');
            byte[] sendData = new byte[sendStrArr.Length];
            try
            {
                for (int i = 0; i < sendStrArr.Length; i++)
                {
                    if (sendStrArr[i].Length != 2)
                    {
                        throw new Exception("数据格式有误,16进制只能由0123456789ABCDEF组成,请输入\"01 02 03 07 0A FE FF\"这种格式的数据!");
                    }
                    sendData[i] = (byte)Convert.ToInt16(sendStrArr[i], 16);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return sendData;

        }

        /// <summary>
        /// 将8位数组转成这种格式"02 A3 1F"字符串
        /// </summary>
        /// <param name="buffer">8位数组</param>
        /// <returns>"02 A3 1F"字符串</returns>
        public static string HexArr2Str(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return "";
            }
            string receiveStr = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                receiveStr += Convert.ToString(buffer[i], 16).PadLeft(2, '0').ToUpper() + " ";
            }

            return receiveStr;
        }
        /// <summary>
        /// 获取所有网卡的ip
        /// </summary>
        /// <returns>ip列表</returns>
        public static List<string> GetIplist()
        {
            List<string> ipList = new List<string>();
            try
            {
                string AddressIP = string.Empty;
                ipList.Add("0.0.0.0");
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        AddressIP = _IPAddress.ToString();
                        ipList.Add(AddressIP);
                    }
                }
                return ipList;
            }
            catch
            {
                return ipList;
                throw;
            }
        }

        /// <summary>
        /// 获取所有的可用串口
        /// </summary>
        /// <returns>com列表</returns>
        public static List<string> GetComlist()
        {
            List<string> comList = new List<string>();
            try
            {
                //System.IO.Ports.SerialPort.GetPortNames()函数功能为获取计算机所有可用串口，以字符串数组形式输出
                string[] ArryPort = System.IO.Ports.SerialPort.GetPortNames(); //定义字符串数组，数组名为 ArryPort，将可用的串口信息存放在字符串中           
                for (int i = 0; i < ArryPort.Length; i++)
                {
                    comList.Add(ArryPort[i]);   //将所有的可用串口号添加到端口对应的组合框中
                }
            }
            catch (Exception)
            {
                throw;
            }
            return comList;

        }
    }
    public class HardwareInfo
    {
        //取机器名
        public static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }
        //取第一块硬盘编号
        public enum NCBCONST
        {
            NCBNAMSZ = 16, /* absolute length of a net name */
            MAX_LANA = 254, /* lana's in range 0 to MAX_LANA inclusive */
            NCBENUM = 0x37, /* NCB ENUMERATE LANA NUMBERS */
            NRC_GOODRET = 0x00, /* good return */
            NCBRESET = 0x32, /* NCB RESET */
            NCBASTAT = 0x33, /* NCB ADAPTER STATUS */
            NUM_NAMEBUF = 30, /* Number of NAME's BUFFER */
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ADAPTER_STATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] adapter_address;
            public byte rev_major;
            public byte reserved0;
            public byte adapter_type;
            public byte rev_minor;
            public ushort duration;
            public ushort frmr_recv;
            public ushort frmr_xmit;
            public ushort iframe_recv_err;
            public ushort xmit_aborts;
            public uint xmit_success;
            public uint recv_success;
            public ushort iframe_xmit_err;
            public ushort recv_buff_unavail;
            public ushort t1_timeouts;
            public ushort ti_timeouts;
            public uint reserved1;
            public ushort free_ncbs;
            public ushort max_cfg_ncbs;
            public ushort max_ncbs;
            public ushort xmit_buf_unavail;
            public ushort max_dgram_size;
            public ushort pending_sess;
            public ushort max_cfg_sess;
            public ushort max_sess;
            public ushort max_sess_pkt_size;
            public ushort name_count;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct NAME_BUFFER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] name;
            public byte name_num;
            public byte name_flags;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct NCB
        {
            public byte ncb_command;
            public byte ncb_retcode;
            public byte ncb_lsn;
            public byte ncb_num;
            public IntPtr ncb_buffer;
            public ushort ncb_length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_callname;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_name;
            public byte ncb_rto;
            public byte ncb_sto;
            public IntPtr ncb_post;
            public byte ncb_lana_num;
            public byte ncb_cmd_cplt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] ncb_reserve;
            public IntPtr ncb_event;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct LANA_ENUM
        {
            public byte length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.MAX_LANA)]
            public byte[] lana;
        }
        [StructLayout(LayoutKind.Auto)]
        public struct ASTAT
        {
            public ADAPTER_STATUS adapt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NUM_NAMEBUF)]
            public NAME_BUFFER[] NameBuff;
        }
        public class Win32API
        {
            [DllImport("NETAPI32.DLL")]
            public static extern char Netbios(ref NCB ncb);
        }
        //取网卡mac
        public static string GetMacAddress()
        {
            string addr = "";
            try
            {
                int cb;
                ASTAT adapter;
                NCB Ncb = new NCB();
                char uRetCode;
                LANA_ENUM lenum;
                Ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                cb = Marshal.SizeOf(typeof(LANA_ENUM));
                Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                Ncb.ncb_length = (ushort)cb;
                uRetCode = Win32API.Netbios(ref Ncb);
                lenum = (LANA_ENUM)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(LANA_ENUM));
                Marshal.FreeHGlobal(Ncb.ncb_buffer);
                if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                    return "";
                for (int i = 0; i < lenum.length; i++)
                {
                    Ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    uRetCode = Win32API.Netbios(ref Ncb);
                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";
                    Ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    Ncb.ncb_callname[0] = (byte)'*';
                    cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) + Marshal.SizeOf(typeof(NAME_BUFFER)) * (int)NCBCONST.NUM_NAMEBUF;
                    Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    Ncb.ncb_length = (ushort)cb;
                    uRetCode = Win32API.Netbios(ref Ncb);
                    adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                    Marshal.FreeHGlobal(Ncb.ncb_buffer);
                    if (uRetCode == (short)NCBCONST.NRC_GOODRET)
                    {
                        if (i > 0)
                            addr += ":";
                        addr = string.Format("{0,2:X}{1,2:X}{2,2:X}{3,2:X}{4,2:X}{5,2:X}",
                        adapter.adapt.adapter_address[0],
                        adapter.adapt.adapter_address[1],
                        adapter.adapt.adapter_address[2],
                        adapter.adapt.adapter_address[3],
                        adapter.adapt.adapter_address[4],
                        adapter.adapt.adapter_address[5]);
                    }
                }
            }
            catch
            {
            }
            return addr.Replace(' ', '0');
        }
    }
    public class IniFiles
    {

        //两个读写ini文件的API

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);


        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);


        public string iniFilePath = "";

        public IniFiles(string iniFilePath = "")
        {
            if (iniFilePath == "")
            {
                iniFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"/config.ini";
            }
            this.iniFilePath = iniFilePath;
        }
        public string Read(string section, string key, string def = "")
        {
            string res = String.Empty;
            try
            {
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, def, sb, 1024, iniFilePath);
                res = sb.ToString();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return res;
        }

        public bool Write(string section, string key, string value)
        {
            bool res = false;
            try
            {
                res = WritePrivateProfileString(section, key, value, iniFilePath);
            }
            catch (Exception)
            {

                throw;
            }
            return res;
            //CheckPath(filePath);
        }
        //删除section 
        public bool Remove(string section, string key = null)
        {
            if (key != null)
            {
                return Write(section, key, null);
            }
            else
            {
                return Write(section, null, null);
            }
        }
    }
    static class Global
    {
        static public string productVersion = System.Windows.Forms.Application.ProductVersion.ToString();
        static public string productName = System.Windows.Forms.Application.ProductName.ToString();
        static public string companyName = System.Windows.Forms.Application.CompanyName.ToString();

        static public IntPtr WINDOW_HANDLER { get { return Win32Gui.FindWindow(null, Console.Title); } }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool AllocConsole();//关联一个控制台


        [DllImport("Kernel32.dll")]
        public static extern void FreeConsole();//释放关联的控制台

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int vKey);

        public static string GetFileSavePath(string defaultDir, string suffixStr)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = defaultDir;

            sfd.Filter = suffixStr;
            //设置默认文件类型显示顺序
            sfd.FilterIndex = 1;
            //保存对话框是否记忆上次打开的目录
            sfd.RestoreDirectory = true;
            //openImageDialog.Multiselect = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(sfd.FileName);
                return sfd.FileName;
            }
            else
            {
                return sfd.FileName;
            }
        }


        /// <summary>
        /// 执行内部命令（cmd.exe 中的命令）
        /// </summary>
        /// <param name="cmdline">命令行</param>
        /// <returns>执行结果</returns>
        public static string ExecuteInCmd(string cmdline)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine(cmdline + "&exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                process.Close();
                return output;
            }
        }

        /// <summary>
        /// 执行外部命令
        /// </summary>
        /// <param name="argument">命令参数</param>
        /// <param name="application">命令程序路径</param>
        /// <returns>执行结果</returns>
        public static string ExecuteOutCmd(string argument, string applocaltion)
        {
            using (var process = new Process())
            {
                process.StartInfo.Arguments = argument;
                process.StartInfo.FileName = applocaltion;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine("exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return output;
            }
        }
        static public string NowTime()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmssff");
        }
    }
    class Lnk
    {
        public static void CreateLnk(string FileName = "", string description = "")
        {
            WriteToTemp(CreateVBS(FileName, description));
            RunProcess();
        }
        //以文件形式写入临时文件夹

        private static string CreateVBS(string fileName = "", string description = "")
        {
            fileName = string.IsNullOrEmpty(fileName) ? Global.productName : fileName;
            string vbs = string.Empty;
            vbs += ("set WshShell = WScript.CreateObject(\"WScript.Shell\")\r\n");
            vbs += ("strDesktop = WshShell.SpecialFolders(\"Desktop\")\r\n");
            vbs += ($"set oShellLink = WshShell.CreateShortcut(strDesktop & \"\\{fileName}.lnk\")\r\n");
            vbs += ($"oShellLink.TargetPath = \"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"\r\n");
            vbs += ($"oShellLink.WindowStyle = 1\r\n");
            vbs += ($"oShellLink.Description = \"{description}\"\r\n");
            vbs += ($"oShellLink.WorkingDirectory = \"{System.Environment.CurrentDirectory}\"\r\n");
            vbs += ("oShellLink.Save");
            return vbs;
        }
        ///
        /// 写入临时文件
        ///
        ///
        private static void WriteToTemp(string vbs)
        {
            if (!string.IsNullOrEmpty(vbs))
            {
                //临时文件
                string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\temp.vbs";
                //写入文件
                FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write);
                try
                {
                    //这里必须用UnicodeEncoding. 因为用UTF-8或ASCII会造成VBS乱码
                    System.Text.UnicodeEncoding uni = new UnicodeEncoding();
                    byte[] b = uni.GetBytes(vbs);
                    fs.Write(b, 0, b.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //MessageBox.Show(ex.Message, "写入临时文件时出现错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Console.WriteLine(tempFile);
                    //释放资源
                    fs.Dispose();
                }
            }
        }
        ///
        /// 执行VBS中的代码
        ///
        private static void RunProcess()
        {
            string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\temp.vbs";
            if (File.Exists(tempFile))
            {
                //执行VBS
                Process.Start(tempFile);
            }
            //File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\temp.vbs");
        }
    }
    // 获取快捷方式目标路径
    class LoopEvent
    {
        bool isRunning = false;
        private Th th = null;
        private Action func = new Action(() => { Thread.Sleep(1); });
        private Action loopFunc = null;
        private ManualResetEvent threadEvent = new ManualResetEvent(false);

        // 公共属性
        static List<LoopEvent> loopEventList = new List<LoopEvent>();
        static Action onAllStart = null;
        static Action onAllEnd = null;
        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool IsRunning { get => isRunning; }
        /// <summary>
        /// 设置所有循环事件状态改变函数
        /// </summary>
        /// <param name="onAllStart"></param>
        /// <param name="onAllEnd"></param>
        public static void SetAllCurFunc(Action onAllStart, Action onAllEnd)
        {
            LoopEvent.onAllStart = onAllStart;
            LoopEvent.onAllEnd = onAllEnd;
        }

        private LoopEvent()
        {
            loopFunc = () =>
            {
                while (true)
                {
                    threadEvent.WaitOne();
                    func();
                }
            };
            th = new Th(loopFunc);
            th.Start();
            loopEventList.Add(this);
        }
        /// <summary>
        /// 传入一个回调
        /// </summary>
        /// <param name="func"></param>
        public LoopEvent(Action func) : this()
        {
            this.func = func;
        }
        /// <summary>
        /// 切换启动和暂停状态
        /// </summary>
        public void CutState()
        {
            isRunning = !isRunning;
            ChangeState();
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="start"></param>
        public void SetState(bool state = false)
        {
            this.isRunning = state;
            ChangeState();
        }
        public void Destroy()
        {
            loopEventList.Remove(this);
        }
        private void ChangeState()
        {
            if (isRunning)
                threadEvent.Set();
            else
                threadEvent.Reset();

            //执行函数开始结束函数
            if (onAllEnd != null && onAllStart != null)
                RunFunc(isRunning);
        }
        /// <summary>
        /// 执行所有都停止所有都开始函数
        /// </summary>
        /// <param name="isRunning"></param>
        private static void RunFunc(bool isRunning)
        {
            int number = 0;
            for (int i = 0; i < loopEventList.Count; i++)
            {
                if (loopEventList[i].IsRunning == true)
                    number++;
            }
            if (number == 1)
            {
                if (isRunning == true)
                    onAllStart();
            }
            if (number == 0)
            {
                if (isRunning == false)
                    onAllEnd();
            }
        }
        /// <summary>
        /// 暂停所有循环事件
        /// </summary>
        public static void StopAll()
        {
            for (int i = 0; i < loopEventList.Count; i++)
            {
                loopEventList[i].SetState(false);
            }
        }
        ~LoopEvent()
        {
            loopEventList.Remove(this);
        }
        static public void StopAllLoopEvent()
        {
            for (int i = 0; i < loopEventList.Count; i++)
            {
                loopEventList[i].SetState(false);
            }
        }
    }

    class LoopEvent_
    {
        bool isRunning = false;
        private Th th = null;
        private Action func = new Action(() => { Thread.Sleep(1); });
        private Action loopFunc = null;

        // 公共属性
        static List<LoopEvent_> loopEventList = new List<LoopEvent_>();
        static Action onAllStart = null;
        static Action onAllEnd = null;
        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool IsRunning { get => isRunning; }
        /// <summary>
        /// 设置所有循环事件状态改变函数
        /// </summary>
        /// <param name="onAllStart"></param>
        /// <param name="onAllEnd"></param>
        public static void SetAllCurFunc(Action onAllStart, Action onAllEnd)
        {
            LoopEvent_.onAllStart = onAllStart;
            LoopEvent_.onAllEnd = onAllEnd;
        }

        private LoopEvent_()
        {
            loopFunc = () =>
            {
                while (true)
                {
                    func();
                }
            };
            loopEventList.Add(this);
        }
        /// <summary>
        /// 传入一个回调
        /// </summary>
        /// <param name="func"></param>
        public LoopEvent_(Action func) : this()
        {
            this.func = func;
        }
        /// <summary>
        /// 切换启动和暂停状态
        /// </summary>
        public void CutState()
        {
            isRunning = !isRunning;
            ChangeState();
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="start"></param>
        public void SetState(bool state = false)
        {
            this.isRunning = state;
            ChangeState();
        }
        public void Destroy()
        {
            if (th != null)
                th.Abort();
            loopEventList.Remove(this);
        }
        private void ChangeState()
        {
            //执行函数开始结束函数
            if (onAllEnd != null && onAllStart != null)
                RunFunc(isRunning);
            if (isRunning)
            {
                th = new Th(loopFunc);
                th.Start();
            }
            else
            {
                if (th != null)
                    th.Abort();
            }
        }
        /// <summary>
        /// 执行所有都停止所有都开始函数
        /// </summary>
        /// <param name="isRunning"></param>
        private static void RunFunc(bool isRunning)
        {
            int number = 0;
            for (int i = 0; i < loopEventList.Count; i++)
            {
                if (loopEventList[i].IsRunning == true)
                {
                    number++;
                    break;
                }
            }
            if (number == 1)
            {
                if (isRunning == true)
                    onAllStart();
            }
            else if (number == 0)
            {
                if (isRunning == false)
                    onAllEnd();
            }
        }
        ~LoopEvent_()
        {
            loopEventList.Remove(this);
        }
        static public void StopAllLoopEvent()
        {
            for (int i = 0; i < loopEventList.Count; i++)
            {
                loopEventList[i].SetState(false);
            }
        }
    }

    class Th
    {
        Thread th = null;

        static List<Th> thList = new List<Th>();

        public System.Threading.ThreadState ThreadState { get => th.ThreadState; }

        public Th(Action func)
        {
            thList.Add(this);
            th = new Thread(new ThreadStart(func));
        }
        public void Start()
        {
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        public void Join()
        {
            th.Join();
        }
        public void Abort()
        {
            if (th != null)
            {
                th.Abort();
                thList.Remove(this);
            }
        }
        public static void AbortAll()
        {
            for (int i = 0; i < thList.Count; i++)
            {
                try
                {
                    if (thList[i] != null)
                        thList[i].Abort();
                }
                catch (ThreadAbortException abortException)
                {
                    Console.WriteLine((string)abortException.ExceptionState);
                }
            }
        }
    }

    class HotKey
    {
        class HK
        {
            public int id;
            public Action func;
            public uint mod;
            public Keys vk;
            public bool run;

            public HK(int id, uint mod, Keys vk, Action func)
            {
                this.id = id;
                this.mod = mod;
                this.vk = vk;
                this.func = func;
                this.run = false;

            }
        }
        private static int count = 0;
        public const int WM_HOTKEY = 0x0312;
        //public const uint None = 0;
        //public const uint Alt = 1;
        //public const uint Ctrl = 2;
        //public const uint Shift = 4;
        //public const uint WindowsKey = 8;

        [DllImport("user32.dll", EntryPoint = "GetMessageA")]
        public static extern bool GetMessageA(out Message lpMsg, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        private static Dictionary<int, HK> hotKeyDic = new Dictionary<int, HK>();



        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(
            IntPtr hWnd,                 //要定义热键的窗口的句柄
            int id,                      //定义热键ID（不能与其它ID重复）         
            uint fsModifiers,    //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                      //定义热键的内容
            );
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd,                 //要取消热键的窗口的句柄
            int id                       //要取消热键的ID
            );
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        /// <summary>
        /// 添加热键
        /// </summary>
        /// <param name="hWnd">注册窗口句柄</param>
        /// <param name="id">热键唯一id</param>
        /// <param name="fsModifiers">功能键</param>
        /// <param name="vk"></param>
        /// <param name="func">回调</param>
        public static int AddHotKey(IntPtr hWnd, FsModifiers fsModifiers, Keys vk, Action func = null)
        {
            return AddHotKey_(hWnd, fsModifiers, vk, func);
        }
        public static int AddHotKey(IntPtr hWnd, uint fsModifiers, Keys vk, Action func = null)
        {
            //byte[] buffer = Guid.NewGuid().ToByteArray();
            //int id = (int)BitConverter.ToInt64(buffer, 0);
            return AddHotKey_(hWnd, fsModifiers, vk, func);
        }

        //控制台模式用下面这个
        public static void AddHotKeyConsole(FsModifiers fsModifiers, Keys vk, Action func = null)
        {
            int id = HotKey.count;
            hotKeyDic.Add(id, new HK(id, (uint)fsModifiers, vk, func));
            HotKey.count++;
        }

        public static int AddHotKey_(IntPtr hWnd, uint fsModifiers, Keys vk, Action func = null)
        {
            int id = HotKey.count;
            if (!RegisterHotKey(hWnd, id, (uint)fsModifiers, vk))
            {
                return -1;
            }
            if (func != null)
            {
                hotKeyDic.Add(id, new HK(id, (uint)fsModifiers, vk, func));
            }
            HotKey.count++;
            return id;
        }


        public static int AddHotKey_(IntPtr hWnd, FsModifiers fsModifiers, Keys vk, Action func = null)
        {
            //byte[] buffer = Guid.NewGuid().ToByteArray();
            int id = HotKey.count;
            if (!RegisterHotKey(hWnd, id, (uint)fsModifiers, vk))
            {
                return -1;
            }
            if (func != null)
            {
                hotKeyDic.Add(id, new HK(id, (uint)fsModifiers, vk, func));
            }
            HotKey.count++;
            return id;
        }
        //public static void AddHotKeyHandler(int id, Action func)
        //{
        //    if (hotKeyDic.ContainsKey(id))
        //    {
        //        hotKeyDic[id] += func;
        //    }
        //    else
        //    {
        //        hotKeyDic.Add(id, new HK(id, fsModifiers, vk, func));
        //    }
        //}
        /// <summary>
        /// 使用热键
        /// </summary>
        /// <param name="m"></param>
        public static bool UseHotKey(Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if (hotKeyDic.ContainsKey(m.WParam.ToInt32()) && hotKeyDic[m.WParam.ToInt32()].run == false)
                {
                    if (hotKeyDic[m.WParam.ToInt32()].func != null)
                    {
                        hotKeyDic[m.WParam.ToInt32()].run = true;
                        hotKeyDic[m.WParam.ToInt32()].func();
                        Thread hotkeyUpThread = new Thread(() =>
                        {
                            while (true)
                            {
                                short state = GetAsyncKeyState((int)hotKeyDic[m.WParam.ToInt32()].vk);
                                if ((state & 0x8000) == 0)
                                {
                                    break;
                                }
                                Thread.Sleep(50);
                            }
                            hotKeyDic[m.WParam.ToInt32()].run = false;
                        });
                        hotkeyUpThread.Start();
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移出热键
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        public static void RemoveHotKey(IntPtr hWnd, int id = -1)
        {
            if (id == -1)
            {
                foreach (var item in hotKeyDic)
                {
                    UnregisterHotKey(hWnd, item.Key);
                }
                hotKeyDic.Clear();
            }
            else
            {
                UnregisterHotKey(hWnd, id);
                hotKeyDic.Remove(id);
            }
        }

        public static void RunInConsole()
        {
            Th t = new Th(() =>
            {
                foreach (var item in hotKeyDic)
                {
                    //Console.WriteLine(item.Key);
                    if (!RegisterHotKey(IntPtr.Zero, item.Key, (uint)item.Value.mod, item.Value.vk))
                    {
                        Console.WriteLine($"{item.Value.mod}+{item.Value.vk},注册失败");
                    }
                }

                Message msg = new Message();
                while (true)
                {
                    if (GetMessageA(out msg, IntPtr.Zero, 0, 0))
                    {
                        HotKey.UseHotKey(msg);
                    }
                }
            });
            t.Start();
        }


        [Flags()]
        public enum FsModifiers : uint
        {
            NONE = 0,
            ALT = 1,
            CTRL = 2,
            SHIFT = 4,
            WIN = 8
        }
    }
    public class Clock
    {
        static Stopwatch stopwatch = Stopwatch.StartNew();
        public static TimeSpan Measure(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
        public static void S(string text = "")
        {
            Console.WriteLine(text);
            stopwatch = Stopwatch.StartNew();
        }

        public static string E(string text = "")
        {
            stopwatch.Stop();
            string s = text + stopwatch.Elapsed.ToString().Remove(12, 4) + "\r\n";
            Console.WriteLine(s);
            return s;
        }
    }
    public class ScForm : Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        public void Init()
        {
            this.components = new System.ComponentModel.Container();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScForm));

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
        }
    }
    public class Sc
    {
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_APPWINDOW = 0x40000;
        private const int WS_EX_TOOLWINDOW = 0x80;
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);





        private const int HORZRES = 8;
        private const int VERTRES = 10;
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
        private const int DESKTOPVERTRES = 117;
        private const int DESKTOPHORZRES = 118;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(
            IntPtr hdc, // handle to DC
            int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        static float sreenScale = GetSreenScale();
        static bool isInScreenshot = false;


        public static Form CanvasForm;

        public static int PhysicsW;
        public static int PhysicsH;


        public static void DrawRect(List<Rectangle> rects)
        {
            if (CanvasForm == null || CanvasForm.IsDisposed)
            {
                CanvasForm = new Form();

                CanvasForm.Load += (s, _e) =>
                {
                    int exStyle = GetWindowLong(CanvasForm.Handle, GWL_EXSTYLE);
                    SetWindowLong(CanvasForm.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
                };


                CanvasForm.FormBorderStyle = FormBorderStyle.None;
                CanvasForm.ShowInTaskbar = false;
                //CanvasForm.Opacity = 0.5;
                CanvasForm.BackColor = Color.White;
                CanvasForm.TopMost = true;
                CanvasForm.WindowState = FormWindowState.Maximized;
                CanvasForm.TransparencyKey = Color.White;
                //form.BackColor = Color.White;
                //form.Cursor = Cursors.Cross;
                //CanvasForm.KeyDown += (s, _e) =>
                //{
                //    if (_e.KeyCode == Keys.Escape)
                //    {
                //        CanvasForm.DialogResult = DialogResult.OK;
                //        CanvasForm.Close();
                //    }
                //};

                CanvasForm.Paint += (s, _e) =>
                {
                    foreach (var rect in rects)
                    {
                        //_e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(27, Color.Black)), rect);
                        //Color customColor = Color.FromArgb(0xBE,0xFF,0x4B);
                        //SolidBrush brush = new SolidBrush(customColor);
                        //Pen pen = new Pen(brush);
                        _e.Graphics.DrawRectangle(Pens.Red, rect);

                    }
                };

                //CanvasForm.MouseUp += (s, _e) =>
                //{
                //    if (_e.Button == MouseButtons.Left)
                //    {
                //        CanvasForm.DialogResult = DialogResult.OK;
                //        CanvasForm.Close();
                //    }
                //};
                CanvasForm.Show();
                //if (CanvasForm.ShowDialog() == DialogResult.OK)
                //{
                //    CanvasForm.Opacity = 0;
                //    CanvasForm.Refresh();
                //    //System.Threading.Thread.Sleep(200); //等待200ms
                //}
            }
            else
            {
                CanvasForm.Close();
                CanvasForm.Dispose();

            }
        }








        public static float GetSreenScale()
        {

            var hdc = GetDC(GetDesktopWindow());
            PhysicsW = GetDeviceCaps(hdc, DESKTOPHORZRES);
            PhysicsH = GetDeviceCaps(hdc, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            float f_Scale = (float)PhysicsW / (float)Screen.PrimaryScreen.Bounds.Width;
            return 1 / f_Scale;
        }

        public static Image CaptureSelectedRegion()
        {
            if (isInScreenshot) return null;
            isInScreenshot = true;
            var startX = 0;
            var startY = 0;
            var width = 0;
            var height = 0;
            try
            {
                using (var bitmap = CaptureScreen())
                {
                    using (var form = new ScForm())
                    {
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.ShowInTaskbar = false;
                        //form.Opacity = 0.5;
                        form.BackColor = Color.Black;
                        form.TopMost = true;
                        form.WindowState = FormWindowState.Maximized;
                        //form.TransparencyKey = Color.White;
                        form.BackgroundImage = bitmap;
                        if(sreenScale != 1)
                            form.BackgroundImageLayout = ImageLayout.Stretch;
                        form.Cursor = Cursors.Cross;
                        form.Init();

                        form.KeyDown += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Escape)
                            {
                                form.DialogResult = DialogResult.OK;
                                form.Close();
                            }
                        };
                        form.MouseDown += (s, e) =>
                        {
                            startX = e.X;
                            startY = e.Y;
                        };

                        form.MouseMove += (s, e) =>
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                width = e.X - startX;
                                height = e.Y - startY;
                                form.Refresh();
                            }
                        };

                        form.Paint += (s, e) =>
                        {
                            if (width != 0 && height != 0)
                            {
                                var rect = new Rectangle(startX, startY, width, height);
                                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, Color.White)), rect);
                                e.Graphics.DrawRectangle(Pens.Blue, rect);
                            }
                        };

                        form.MouseUp += (s, e) =>
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                form.DialogResult = DialogResult.OK;
                                form.Close();
                            }
                        };
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            form.Opacity = 0;
                            form.Refresh();
                            //System.Threading.Thread.Sleep(200); //等待200ms
                            isInScreenshot = false;
                            return CropBitmap(bitmap, new Rectangle((int)(startX / sreenScale), (int)(startY / sreenScale), (int)(width / sreenScale), (int)(height / sreenScale)));
                            //return bitmap.Clone(new Rectangle((int)(startX / sreenScale), (int)(startY / sreenScale), (int)(width / sreenScale), (int)(height / sreenScale)), bitmap.PixelFormat);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            isInScreenshot = false;
            return null;
        }

        public static Bitmap CropBitmap(Bitmap originalBitmap, Rectangle cropRect)
        {
            // 创建新的 Bitmap 对象，并在其中绘制裁剪后的图像
            Bitmap croppedBitmap = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(originalBitmap, new Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel);
            }
            // 返回裁剪后的图像
            return croppedBitmap;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern int SaveBitmap(IntPtr hBitmap, string lpFileName);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        private enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        public static Bitmap CaptureScreen()
        {
            // 获取屏幕句柄和设备上下文
            IntPtr desktopHandle = GetDesktopWindow();
            IntPtr desktopDC = GetWindowDC(desktopHandle);

            // 获取屏幕大小
            int screenWidth = PhysicsW;
            int screenHeight = PhysicsH;

            // 创建位图对象
            Bitmap bitmap = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // 创建画布对象
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 复制屏幕内容到位图上
                IntPtr bitmapDC = graphics.GetHdc();
                BitBlt(bitmapDC, 0, 0, screenWidth, screenHeight, desktopDC, 0, 0, TernaryRasterOperations.SRCCOPY);
                graphics.ReleaseHdc(bitmapDC);
            }

            DeleteDC(desktopDC);

            // 释放设备上下文
            ReleaseDC(desktopHandle, desktopDC);

            return bitmap;
        }

    }
}
namespace VirtualKey
{
    //[StructLayout(LayoutKind.Explicit)]
    //public struct INPUT
    //{
    //    [FieldOffset(0)]
    //    public int type;
    //    [FieldOffset(4)]
    //    public KEYBDINPUT ki;
    //    [FieldOffset(4)]
    //    public MOUSEINPUT mi;
    //    [FieldOffset(4)]
    //    public HARDWAREINPUT hi;
    //}


    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdHardwareInputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public MouseKeybdHardwareInputUnion mkhi;
    }

    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
        public MOUSEINPUT(int dx, int dy, int mouseData, int dwFlags, int time, IntPtr dwExtraInfo)
        {
            this.dx = dx;
            this.dy = dy;
            this.mouseData = mouseData;
            this.dwFlags = dwFlags;
            this.time = time;
            this.dwExtraInfo = dwExtraInfo;
        }
    }
    public struct KEYBDINPUT
    {
        public short wVk;
        public short wScan;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;

        public KEYBDINPUT(short wVk, short wScan, int dwFlags, int time, IntPtr dwExtraInfo)
        {
            this.wVk = wVk;
            this.wScan = wScan;
            this.dwFlags = dwFlags;
            this.time = time;
            this.dwExtraInfo = dwExtraInfo;
        }
    }
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;

        public HARDWAREINPUT(int uMsg, short wParamL, short wParamH)
        {
            this.uMsg = uMsg;
            this.wParamL = wParamL;
            this.wParamH = wParamH;
        }
    }

    //[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    //public struct Point
    //{

    //    /// LONG->int
    //    public int x;

    //    /// LONG->int
    //    public int y;

    //    public Point(int x, int y)
    //    {
    //        this.x = x;
    //        this.y = y;
    //    }
    //}

    class DirectInput
    {
        public enum MouseKey : int
        {
            LEFT = 0x0002,
            RIGHT = 0x0008,
            MIDDLE = 0x0020,
        }

        // 输入类型
        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;

        // 映射虚拟键映射类型
        const int MAPVK_VK_TO_CHAR = 2;
        const int MAPVK_VK_TO_VSC = 0;
        const int MAPVK_VSC_TO_VK = 1;
        const int MAPVK_VSC_TO_VK_EX = 3;

        // 鼠标扫码映射
        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_LEFTCLICK = MOUSEEVENTF_LEFTDOWN + MOUSEEVENTF_LEFTUP;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int MOUSEEVENTF_RIGHTCLICK = MOUSEEVENTF_RIGHTDOWN + MOUSEEVENTF_RIGHTUP;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_MIDDLECLICK = MOUSEEVENTF_MIDDLEDOWN + MOUSEEVENTF_MIDDLEUP;
        const int MOUSEEVENTF_WHEEL = 0x800;

        // 按键输入标志
        const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int KEYEVENTF_KEYUP = 0x0002;
        const int KEYEVENTF_SCANCODE = 0x0008;
        const int KEYEVENTF_UNICODE = 0x0004;
        public static Dictionary<string, string> needShiftKey = new Dictionary<string, string>(){
             {"!","1"},
             {"@","2"},
             {"#","3"},
             {"$","4"},
             {"%","5"},
             {"^","6"},
             {"&","7"},
             {"*","8"},
             {"(","9"},
             {")","0"},
             {"_","-"},
             {"+","="},


             {"A","a"},
             {"B","b"},
             {"C","c"},
             {"D","d"},
             {"E","e"},
             {"F","f"},
             {"G","g"},
             {"H","h"},
             {"I","i"},
             {"J","j"},
             {"K","k"},
             {"L","l"},
             {"M","m"},
             {"N","n"},
             {"O","o"},
             {"P","p"},
             {"Q","q"},
             {"R","r"},
             {"S","s"},
             {"T","t"},
             {"U","u"},
             {"V","v"},
             {"W","w"},
             {"X","x"},
             {"Y","y"},
             {"Z","z"},

             {"{","["},
             {"}","]"},
             {"|","\\"},
             {":",";"},
             {"\"","'"},
             {"<",","},
             {">","."},
             {"?","/"},
        };
        //键盘扫描码映射
        public static Dictionary<string, short> keyScanCodeMappings = new Dictionary<string, short>(){
            //第一行
            {"escape",0x01},
            {"esc",0x01},
            {"f1",0x3B},
            {"f2",0x3C},
            {"f3",0x3D},
            {"f4",0x3E},
            {"f5",0x3F},
            {"f6",0x40},
            {"f7",0x41},
            {"f8",0x42},
            {"f9",0x43},
            {"f10",0x44},
            {"f11",0x57},
            {"f12",0x58},

            //截图键
            {"printscreen",0xB7},
            {"prntscrn",0xB7},
            {"prtsc",0xB7},
            {"prtscr",0xB7},

            {"scrolllock",0x46},
            {"scroll",0x46},


            {"pause",0xC5},
            
            
            //第二行
            {"oemtilde",0x29},
            {"`",0x29},

            {"d1",0x02},
            {"d2",0x03},
            {"d3",0x04},
            {"d4",0x05},
            {"d5",0x06},
            {"d6",0x07},
            {"d7",0x08},
            {"d8",0x09},
            {"d9",0x0A},
            {"d0",0x0B},

            {"1",0x02},
            {"2",0x03},
            {"3",0x04},
            {"4",0x05},
            {"5",0x06},
            {"6",0x07},
            {"7",0x08},
            {"8",0x09},
            {"9",0x0A},
            {"0",0x0B},

            {"-",0x0C},
            {"oemminus",0x0C},

            {"=",0x0D},
            {"oemplus",0x0D},

            {"back",0x0E},
            {"backspace",0x0E},

            {"insert",0xD2 + 1024},

            {"home",0xC7 + 1024},

            {"pageup",0xC9 + 1024},
            {"prior",0xC9 + 1024},

            {"pagedown",0xD1 + 1024},

            // numpad
            {"numlock",0x45},
            {"divide",0xB5 + 1024},
            {"multiply",0x37},
            {"subtract",0x4A},
            {"add",0x4E},
            {"decimal",0x53},

            {"numpad9",0x49},
            {"numpad8",0x48},
            {"numpad7",0x47},
            {"numpad6",0x4d},
            {"numpad5",0x4c},
            {"numpad4",0x4b},
            {"numpad3",0x51},
            {"numpad2",0x50},
            {"numpad1",0x4F},
            {"numpad0",0x52},

            // KEY_NUMPAD_ENTER,0x9C + 1024},
            // KEY_NUMPAD_1,0x4F},
            // KEY_NUMPAD_2,0x50},
            // KEY_NUMPAD_3,0x51},
            // KEY_NUMPAD_4,0x4B},
            // KEY_NUMPAD_5,0x4C},
            // KEY_NUMPAD_6,0x4D},
            // KEY_NUMPAD_7,0x47},
            // KEY_NUMPAD_8,0x48},
            // KEY_NUMPAD_9,0x49},
            // KEY_NUMPAD_0,0x52},
            // end numpad
            //第三行
            {"tab",0x0F},
            {"q",0x10},
            {"w",0x11},
            {"e",0x12},
            {"r",0x13},
            {"t",0x14},
            {"y",0x15},
            {"u",0x16},
            {"i",0x17},
            {"o",0x18},
            {"p",0x19},
            {"[",0x1A},
            {"oem4",0x1A},

            {"]",0x1B},
            {"oem6",0x1B},

            {"\\",0x2B},
            {"oempipe",0x2B},

            {"del",0xD3 + 1024},
            {"delete",0xD3 + 1024},
            {"end",0xCF + 1024},

            //第四行
            {"capslock",0x3A},
            {"a",0x1E},
            {"s",0x1F},
            {"d",0x20},
            {"f",0x21},
            {"g",0x22},
            {"h",0x23},
            {"j",0x24},
            {"k",0x25},
            {"l",0x26},

            {"oemsemicolon",0x27},
            {";",0x27},

            {"oemquotes",0x28},
            {"'",0x28},

            {"enter",0x1C},


            //第五行
            {"shift",0x2A},
            {"shiftleft",0x2A},
            {"lshiftkey",0x2A},

            {"z",0x2C},
            {"x",0x2D},
            {"c",0x2E},
            {"v",0x2F},
            {"b",0x30},
            {"n",0x31},
            {"m",0x32},

            {"oemcomma",0x33},
            {",",0x33},

            {"oemperiod",0x34},
            {".",0x34},

            {"oem2",0x35},
            {"/",0x35},

            {"rshiftkey",0x36},
            {"shiftright",0x36},

            //第六行
            
            {"ctrl",0x1D},
            {"ctrlleft",0x1D},
            {"lcontrolkey",0x1D},

            {"win",0x5B},
            {"lwin",0x5B},
            {"win2",0xDB + 1024},
            {"win3",0x5B + 1024},
            {"winleft",0xDB + 1024},

            {"alt",0x38},
            {"altleft",0x38},
            {"lmenu",0x38},

            {"space",0x39},
            {" ",0x39},

            {"altright",0xB8 + 1024},
            {"rmenu",0xB8 + 1024},

            {"winright",0xDC + 1024},
            {"apps",0xDD + 1024},

            {"ctrlright",0x9D + 1024},
            {"rcontrolkey",0x9D + 1024},

            
            // arrow key scancodes can be different depending on the hardware},
            // so I think the best solution is to look it up based on the virtual key
            // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya?redirectedfrom=MSDN
            {"up",MapVirtualKeyW(0x26, MAPVK_VK_TO_VSC)},
            {"left", MapVirtualKeyW(0x25, MAPVK_VK_TO_VSC)},
            {"down", MapVirtualKeyW(0x28, MAPVK_VK_TO_VSC)},
            {"right",MapVirtualKeyW(0x27, MAPVK_VK_TO_VSC)},
        };

        /// Return Type: UINT->unsigned int
        /// uCode: UINT->unsigned int
        /// uMapType: UINT->unsigned int
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MapVirtualKeyW")]
        public static extern short MapVirtualKeyW(uint uCode, uint uMapType);

        /// Return Type: LPARAM->LONG_PTR->int
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetMessageExtraInfo")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.SysInt)]
        public static extern IntPtr GetMessageExtraInfo();

        /// Return Type: int
        ///nIndex: int
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int nIndex);



        /// Return Type: BOOL->int
        ///lpPoint: LPPOINT->tagPOINT*
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool GetCursorPos([System.Runtime.InteropServices.OutAttribute()] out Point lpPoint);

        /// Return Type: SHORT->short
        ///nVirtKey: int
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetKeyState")]
        public static extern short GetKeyState(int nVirtKey);



        [DllImport("user32")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="c"></param>
        public static void KeyDown(string key)
        {
            key = key.ToLower();
            INPUT[] input = new INPUT[1];
            int keybdFlags = KEYEVENTF_SCANCODE;
            if (key.IndexOf("win") != -1)
            {
                input[0].type = INPUT_KEYBOARD;
                input[0].mkhi.ki.wVk = keyScanCodeMappings[key];
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            }
            else
            {
                //int keybdFlags = 0;
                if (key == "up" || key == "down" || key == "left" || key == "right")
                {
                    keybdFlags |= KEYEVENTF_EXTENDEDKEY;
                    if (GetKeyState(0x90) != 0)
                    {
                        input[0].type = INPUT_KEYBOARD;
                        input[0].mkhi.ki = new KEYBDINPUT(0, 0xE0, KEYEVENTF_SCANCODE, 0, GetMessageExtraInfo());
                        SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
                    }
                }
                input[0].type = INPUT_KEYBOARD;
                //input[0].ki.wVk = keyScanCodeMappings[key];
                input[0].mkhi.ki = new KEYBDINPUT(0, keyScanCodeMappings[key], keybdFlags, 0, GetMessageExtraInfo());
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            }
        }
        public static void KeyDown111()
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_KEYBOARD;
            input[0].mkhi.ki.wVk = 0x26;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));

            input[0].type = INPUT_KEYBOARD;
            input[0].mkhi.ki.wVk = 0x26;
            input[0].mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
        }

        /// <summary>
        /// 弹起按键
        /// </summary>
        /// <param name="key"></param>
        public static void KeyUp(string key)
        {
            key = key.ToLower();
            int keybdFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;
            //int keybdFlags = KEYEVENTF_KEYUP;
            INPUT[] input = new INPUT[1];
            if (key.IndexOf("win") != -1)
            {
                input[0].type = INPUT_KEYBOARD;
                input[0].mkhi.ki.wVk = keyScanCodeMappings[key];
                input[0].mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            }
            else
            {
                if (key == "up" || key == "down" || key == "left" || key == "right")
                {
                    keybdFlags |= KEYEVENTF_EXTENDEDKEY;
                }
                input[0].type = INPUT_KEYBOARD;
                //input[0].ki.wVk = keyScanCodeMappings[key];
                //input[0].ki.dwFlags = KEYEVENTF_KEYUP;
                input[0].mkhi.ki = new KEYBDINPUT(0, keyScanCodeMappings[key], keybdFlags, 0, GetMessageExtraInfo());
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));


                if (key == "up" || key == "down" || key == "left" || key == "right")
                {
                    if (GetKeyState(0x90) != 0)
                    {
                        input[0].type = INPUT_KEYBOARD;
                        input[0].mkhi.ki = new KEYBDINPUT(0, 0xE0, KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP, 0, GetMessageExtraInfo());
                        SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
                    }
                }
            }



        }

        /// <summary>
        /// 按下并弹起按键
        /// </summary>
        /// <param name="key"></param>
        public static void KeyPerss(string key, int delay = 0)
        {
            key = key.ToLower();
            KeyDown(key);
            Thread.Sleep(delay);
            KeyUp(key);
        }
        public static Point GetMousePositon()
        {
            Point cursor;
            GetCursorPos(out cursor);
            return cursor;
        }
        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Point Position(int x = -1, int y = -1)
        {
            if (x == -1 && y == -1)
            {
                Point cursor;
                GetCursorPos(out cursor);
                return cursor;
            }
            else
            {
                return new Point(x, y);
            }
        }
        /// <summary>
        /// 获取显示宽度
        /// </summary>
        /// <returns></returns>
        private static Point Display()
        {
            return new Point(GetSystemMetrics(0), GetSystemMetrics(1));
        }
        /// <summary>
        /// 计算偏移
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static Point ToWindowsCoordinates(int x = 0, int y = 0)
        {
            int display_width = Display().X;
            int display_height = Display().Y;

            int windowsX = (int)Math.Floor((double)(x * 65536) / (double)display_width) + 1;
            int windowsY = (int)Math.Floor((double)(y * 65536) / (double)display_height) + 1;

            return new Point(windowsX, windowsY);
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="relative">是否是相对位置</param>
        public static void MouseMoveTo(int x, int y, bool relative = false)
        {
            if (!relative)
            {
                Point cursor = Position(x, y);
                int currentX = cursor.X;
                int currentY = cursor.Y;
                int xx = ToWindowsCoordinates(currentX, currentY).X;
                int yy = ToWindowsCoordinates(currentX, currentY).Y;
                //Console.WriteLine($"{xx},{yy},fffffff{ GetMessageExtraInfo()}");

                INPUT[] input = new INPUT[1];
                input[0].type = INPUT_MOUSE;


                input[0].mkhi.mi = new MOUSEINPUT(xx, yy, 0, MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, 0, GetMessageExtraInfo());
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));

            }
            else
            {
                Point cursor = Position();
                int currentX = cursor.X;
                int currentY = cursor.Y;

                int xx = ToWindowsCoordinates(currentX + x, currentY + y).X;
                int yy = ToWindowsCoordinates(currentX + x, currentY + y).Y;

                INPUT[] input = new INPUT[1];
                input[0].type = INPUT_MOUSE;


                input[0].mkhi.mi = new MOUSEINPUT(xx, yy, 0, MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, 0, GetMessageExtraInfo());
                SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            }
        }
        public static void MouseDown(MouseKey key, int x = -1, int y = -1)
        {
            MouseDown((int)key, x, y);
        }
        public static void MouseUp(MouseKey key, int x = -1, int y = -1)
        {
            MouseUp((int)key, x, y);
        }
        public static void MouseClick(MouseKey key, int x = -1, int y = -1, bool relative = false, int delay = 0)
        {
            MouseClick((int)key, x, y, relative, delay);
        }

        private static void MouseClick(int key, int x = -1, int y = -1, bool relative = false, int delay = 0)
        {
            if (x != -1 && y != -1)
            {
                MouseMoveTo(x, y, relative);
            }
            MouseDown(key);
            Thread.Sleep(delay);
            MouseUp(key);
        }

        private static void MouseDown(int key, int x = -1, int y = -1)
        {
            if (x != -1 && y != -1)
                MouseMoveTo(x, y);
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].mkhi.mi.dwFlags = key;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
        }
        private static void MouseUp(int key, int x = -1, int y = -1)
        {
            if (x != -1 && y != -1)
                MouseMoveTo(x, y);
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].mkhi.mi.dwFlags = key * 2;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
        }
        public static void MouseClick(int key, int x = -1, int y = -1, bool relative = false)
        {
            if (x != -1 && y != -1)
            {
                MouseMoveTo(x, y, relative);
            }
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].mkhi.mi.dwFlags = key + key * 2;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
        }


        /// <summary>
        /// 滚动
        /// </summary>
        /// <param name="val"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="relative"></param>
        private static void _MouseWheel(int val, int x = -1, int y = -1, bool relative = false)
        {
            if (x != -1 && y != -1)
            {
                MouseMoveTo(x, y, relative);
            }
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].mkhi.mi.dwFlags = MOUSEEVENTF_WHEEL;
            input[0].mkhi.mi.mouseData = val * 120;
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
        }
        public static void MouseWheel(int val, int x = -1, int y = -1, bool relative = false, int delay = 0)
        {
            for (int c = 0; c < Math.Abs(val); c++)
            {
                _MouseWheel(Math.Sign(val));
                Thread.Sleep(delay);
            }
        }
    }



    class LG
    {
        [DllImport("ghub_device.dll", EntryPoint = "device_open")]
        public static extern bool DeviceOpen();
        [DllImport("ghub_device.dll", EntryPoint = "device_close")]
        public static extern bool DeviceClose();
        static bool deviceOpenFlag = DeviceOpen();
        [DllImport("ghub_device.dll", EntryPoint = "moveR")]
        private static extern bool MouseMove_(int x, int y, int absMove = 1);


        [DllImport("ghub_device.dll", EntryPoint = "key_down")]
        public static extern void KeyDown(string a);

        [DllImport("ghub_device.dll", EntryPoint = "key_up")]
        public static extern void KeyUp(string a);

        [DllImport("ghub_device.dll", EntryPoint = "mouse_down")]
        public static extern void MouseDown(int a);

        [DllImport("ghub_device.dll", EntryPoint = "mouse_up")]
        public static extern void MouseUp(int a);



        public static void MouseMove(int x, int y, int absMove)
        {

            Point cursor;
            DirectInput.GetCursorPos(out cursor);
            if (absMove == 1)
            {
                x = x - cursor.X;
                y = y - cursor.Y;
            }
            MouseMove_(x, y);
        }

    }
}


namespace KMHook
{
    class MouseHook
    {
        #region 常量
        public const int WM_MOUSEMOVE = 0x200; // 鼠标移动
        public const int WM_LBUTTONDOWN = 0x201;// 鼠标左键按下
        public const int WM_RBUTTONDOWN = 0x204;// 鼠标右键按下
        public const int WM_MBUTTONDOWN = 0x207;// 鼠标中键按下
        public const int WM_LBUTTONUP = 0x202;// 鼠标左键抬起
        public const int WM_RBUTTONUP = 0x205;// 鼠标右键抬起
        public const int WM_MBUTTONUP = 0x208;// 鼠标中键抬起
        public const int WM_LBUTTONDBLCLK = 0x203;// 鼠标左键双击
        public const int WM_RBUTTONDBLCLK = 0x206;// 鼠标右键双击
        public const int WM_MBUTTONDBLCLK = 0x209;// 鼠标中键双击
        public const int WM_MOUSEWHEEL = 0x020A;// 鼠标滚轮滚动
        public const int WH_MOUSE_LL = 14; //可以截获整个系统所有模块的鼠标事件。

        #endregion

        #region 成员变量、回调函数、事件
        /// <summary>
        /// 钩子回调函数
        /// </summary>
        /// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且应返回CallNextHookEx返回的值。此参数可以是下列值之一。(来自官网手册)</param>
        /// <param name="wParam">记录了按下的按钮</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        /// <summary>
        /// 全局的鼠标事件
        /// </summary>
        /// <param name="wParam"> 代表发生的鼠标的事件 </param>
        /// <param name="mouseMsg">钩子的结构体，存储着鼠标的位置及其他信息</param>
        public delegate void MyMouseEventHandler(Int32 wParam, MouseHookStruct mouseMsg);
        private event MyMouseEventHandler OnMouseActivity;
        // 声明鼠标钩子事件类型
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄
        // 锁
        private readonly object lockObject = new object();
        // 当前状态,是否已经启动
        private bool isStart = false;
        #endregion

        #region Win32的API
        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt; // 鼠标位置
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        //声明一个Point的封送类型  
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }



        // 装置钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        #endregion


        #region 构造(单例模式)与析构函数
        private static volatile MouseHook MyMouseHook;
        private readonly static object createLock = new object();
        private MouseHook() { }

        public static MouseHook GetMouseHook()
        {
            if (MyMouseHook == null)
            {
                lock (createLock)
                {
                    if (MyMouseHook == null)
                    {
                        MyMouseHook = new MouseHook();
                    }
                }
            }
            return MyMouseHook;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MouseHook()
        {
            Stop();
        }
        #endregion


        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void Start()
        {
            if (isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (isStart)
                {
                    return;
                }
                if (OnMouseActivity == null)
                {
                    throw new Exception("Please set handler first!Then run Start");
                }
                // 安装鼠标钩子
                if (_hMouseHook == 0)
                {
                    // 生成一个HookProc的实例.
                    _mouseHookProcedure = new HookProc(MouseHookProc);
                    _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                    //假设装置失败停止钩子
                    if (_hMouseHook == 0)
                    {
                        Stop();
                        throw new Exception("SetWindowsHookEx failed.");
                    }
                }
                isStart = true;
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void Stop()
        {
            if (!isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (!isStart)
                {
                    return;
                }
                bool retMouse = true;
                if (_hMouseHook != 0)
                {
                    retMouse = UnhookWindowsHookEx(_hMouseHook);
                    _hMouseHook = 0;
                }
                // 假设卸下钩子失败
                if (!(retMouse))
                    throw new Exception("UnhookWindowsHookEx failed.");
                // 删除所有事件
                OnMouseActivity = null;
                // 标志位改变
                isStart = false;
            }
        }
        bool intercept = false;
        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            intercept = false;
            // 假设正常执行而且用户要监听鼠标的消息
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                OnMouseActivity(wParam, MyMouseHookStruct);
            }
            if (intercept)
            {
                return 1;
            }
            else
            {
                return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
            }
            // 启动下一次钩子
        }
        /// <summary>
        /// 是否拦截
        /// </summary>
        /// <param name="nextHook"></param>
        public void InterceptThisHook(bool nextHook)
        {
            this.intercept = nextHook;
        }

        /// <summary>
        /// 注册全局鼠标事件
        /// </summary>
        /// <param name="handler"></param>
        public void AddMouseHandler(MyMouseEventHandler handler)
        {
            OnMouseActivity += handler;
        }

        /// <summary>
        /// 注销全局鼠标事件
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveMouseHandler(MyMouseEventHandler handler)
        {
            if (OnMouseActivity != null)
            {
                OnMouseActivity -= handler;
            }
        }
    }

    class KeyboardHook
    {
        #region 常数和结构
        #region wParam对应的按钮事件
        public const int WM_KEYDOWN = 0x100;    // 键盘被按下
        public const int WM_KEYUP = 0x101;      // 键盘被松开
        public const int WM_SYSKEYDOWN = 0x104; // 键盘被按下，这个是系统键被按下，例如Alt、Ctrl等键
        public const int WM_SYSKEYUP = 0x105;   // 键盘被松开，这个是系统键被松开，例如Alt、Ctrl等键
        #endregion
        public const int WH_KEYBOARD_LL = 13;


        [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
        public class KeyboardHookStruct

        {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码 
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region 成员变量、委托、事件
        private static int hHook;
        private static HookProc KeyboardHookDelegate;
        // 键盘回调委托
        public delegate void KeyboardHandler(Int32 wParam, KeyboardHookStruct
keyboardHookStruct);
        // 键盘回调事件
        private static event KeyboardHandler Handlers;
        // 锁
        private readonly object lockObject = new object();
        // 当前状态,是否已经启动
        private volatile bool isStart = false;
        #endregion

        #region Win32的Api
        private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //安装钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention =
CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr
hInstance, int threadId);

        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention =
CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention =
CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam,
IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention =
CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region 单例模式
        private static volatile KeyboardHook MyKeyboard;
        private readonly static object createLock = new object();
        private KeyboardHook() { }
        public static KeyboardHook GetKeyboardHook()
        {
            if (MyKeyboard == null)
            {
                lock (createLock)
                {
                    if (MyKeyboard == null)
                    {
                        MyKeyboard = new KeyboardHook();
                    }
                }
            }
            return MyKeyboard;
        }
        #endregion

        /// <summary>
        /// 安装钩子
        /// </summary>
        public void Start()
        {
            if (isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (isStart)
                {
                    return;
                }
                if (Handlers == null)
                {
                    throw new Exception("Please set handler first!Then run Start");
                }
                KeyboardHookDelegate = new HookProc(KeyboardHookProc);
                Process cProcess = Process.GetCurrentProcess();
                ProcessModule cModule = cProcess.MainModule;
                var mh = GetModuleHandle(cModule.ModuleName);
                hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
                isStart = true;
            }
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void Stop()
        {
            if (!isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (!isStart)
                {
                    return;
                }
                UnhookWindowsHookEx(hHook);
                // 清除所有事件
                Handlers = null;
                isStart = false;
            }
        }

        /// <summary>
        /// 键盘的系统回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            if ((nCode >= 0) && Handlers != null)
            {
                KeyboardHookStruct KeyDataFromHook =
(KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Handlers(wParam, KeyDataFromHook);
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 添加按键的回调函数
        /// </summary>
        /// <param name="handler"></param>
        public void AddKeyboardHandler(KeyboardHandler handler)
        {
            Handlers += handler;
        }

        /// <summary>
        /// 删除指定按键的回调函数
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveKeyboardHandler(KeyboardHandler handler)
        {
            if (Handlers != null)
            {
                Handlers -= handler;
            }
        }
    }


}
