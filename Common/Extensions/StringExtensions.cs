
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    #region MD5

    /// <summary>
    /// 用MD5加密字符串，可选择生成16位或者32位的加密字符串
    /// </summary>
    /// <param name="str">待加密的字符串</param>
    /// <param name="bit">位数，一般取值16 或 32</param>
    /// <returns>返回的加密后的字符串</returns>
    public static string ToMD5(this string str, int bit = 32)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(str));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        if (bit == 16)
            return tmp.ToString().Substring(8, 16);
        else
        if (bit == 32) return tmp.ToString();//默认情况
        else return string.Empty;
    }

    ///// <summary>
    ///// 用MD5加密字符串  不适用安卓打包
    ///// </summary>
    ///// <param name="str">待加密的字符串</param>
    ///// <returns></returns>
    //public static string MD5Encrypt(string str)
    //{
    //    MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
    //    byte[] hashedDataBytes;
    //    hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(str));
    //    StringBuilder tmp = new StringBuilder();
    //    foreach (byte i in hashedDataBytes)
    //    {
    //        tmp.Append(i.ToString("x2"));
    //    }
    //    return tmp.ToString();
    //}

    /// <summary>
    /// 用MD5加密字符串 适用安卓打包
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToMD5(this string str)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    #endregion

    #region string全角半角转换

    /// <summary>
    /// 转全角(SBC case)
    /// </summary>
    /// <param name="input">任意字符串</param>
    /// <returns>全角字符串</returns>
    public static string ToSBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }
    /// <summary>
    /// 转半角(DBC case)
    /// </summary>
    /// <param name="input">任意字符串</param>
    /// <returns>半角字符串</returns>
    public static string ToDBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }

    #endregion

    #region IsNullOrEmpty

    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static bool NotEmpty(this string s)
    {
        return s.IsNullOrEmpty() ? false : true;
    }

    #endregion

    #region string和int类型的转换

    public static bool IsInt(this string s)
    {
        int i;
        return int.TryParse(s, out i);
    }

    public static int AsInt(this string s)
    {
        //针对doule类型的转换
        if (s.IndexOf('.') > -1)
        {
            double d = double.Parse(s);
            return Convert.ToInt32(d);
        }
        return int.Parse(s);
    }

    public static ushort ToUShort(this string s)
    {
        return ushort.Parse(s);
    }

    #endregion

    #region string 首字母大小写

    public static string ToCamel(this string s)
    {
        if (IsNullOrEmpty(s)) return s;
        return s[0].ToString().ToLower() + s.Substring(1);
    }

    public static string ToPascal(this string s)
    {
        if (IsNullOrEmpty(s)) return s;
        return s[0].ToString().ToUpper() + s.Substring(1);
    }

    #endregion

    #region 字符串截取

    public static string ToSub(this string s, char value, char startChar = '.')
    {
        if (s.IndexOf(value) > 0)
        {
            return s.Substring(s.IndexOf(startChar) + 1, s.IndexOf(value) - s.IndexOf(startChar) - 1);
        }

        return string.Empty;
    }

    #endregion

    #region int 类型的公式值 计算

    public static object ToIntCalculation(this string s)
    {
        return new DataTable().Compute(s, "");
    }

    #endregion

    #region 编码间转换

    /// <summary>
    /// GB2312转换成UTF8
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string gb2312_utf8(this string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        byte[] gb;
        gb = gb2312.GetBytes(text);
        gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
        //返回转换后的字符   
        return utf8.GetString(gb);
    }

    /// <summary>
    /// UTF8转换成GB2312
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string utf8_gb2312(this string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");

        byte[] utf;
        utf = utf8.GetBytes(text);
        utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
        //返回转换后的字符   
        return gb2312.GetString(utf);
    }
    /// <summary>
    /// UTF8转ASCII
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string utf8_asscii(this string text)
    {
        System.Text.Encoding asscii, utf8;
        asscii = System.Text.Encoding.GetEncoding("ascii");
        utf8 = System.Text.Encoding.GetEncoding("utf-8");

        byte[] utf;
        utf = utf8.GetBytes(text);
        utf = System.Text.Encoding.Convert(utf8, asscii, utf);

        return asscii.GetString(utf);
    }
    /// <summary>
    /// ASCII转UTF8
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ascii_utf8(this string text)
    {
        System.Text.Encoding asscii, utf8;
        asscii = System.Text.Encoding.GetEncoding("ascii");
        utf8 = System.Text.Encoding.GetEncoding("utf-8");

        byte[] utf;
        utf = asscii.GetBytes(text);
        utf = System.Text.Encoding.Convert(asscii, utf8, utf);

        return utf8.GetString(utf);
    }

    /// <summary>
    /// UNICODE转String
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string str_unicode(this string text)
    {
        byte[] b = Encoding.Unicode.GetBytes(text);
        string str = "";
        foreach (var x in b)
        {
            str += string.Format("{0:X2}", x) + " ";
        }
        return str;
    }

    /// <summary>
    /// String 转UNICODE
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string unicode_str(this string text)
    {
        string cd2 = text.Replace("+", "");
        cd2 = cd2.Replace("\r", "");
        cd2 = cd2.Replace("\n", "");
        cd2 = cd2.Replace("\r\n", "");
        cd2 = cd2.Replace("\t", "");
        if (cd2.Length % 4 != 0)
        {
            return "Unicode编码为双字节，请删多或补少！确保是二的倍数。";
        }
        else
        {
            int len = cd2.Length / 2;
            byte[] b = new byte[len];
            for (int i = 0; i < cd2.Length; i += 2)
            {
                string bi = cd2.Substring(i, 2);
                b[i / 2] = (byte)Convert.ToInt32(bi, 16);
            }
            string str = Encoding.Unicode.GetString(b);
            return str;
        }
    }

    #endregion

    #region 字符串计算

    /// <summary>
    /// 字符串公式计算
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int CalcByTable(this string expression, string[] value)
    {
        DataTable table = new DataTable();

        string condition = string.Empty;

        for (int i = 0; i < value.Length; i++)
        {
            switch (i)
            {
                case 0:
                    if (value[0] != "-1")
                        condition = expression.Replace("x", value[0]);
                    break;
                case 1:
                    if (value[1] != "-1")
                        condition = condition.Replace("y", value[1]);
                    break;
                case 2:
                    if (value[2] != "-1")
                        condition = condition.Replace("z", value[2]);
                    break;
                default:
                    break;
            }
        }

        string num = table.Compute(condition, "false").ToString();
        int result = num.AsInt();
        return result;
    }

    #endregion

    #region 汉字拼音转换

    //定义拼音区编码数组
    private static int[] getValue = new int[]
        {
                -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
                -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
                -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
                -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
                -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
                -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
                -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
                -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
                -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
                -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
                -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
                -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
                -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
                -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
                -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
                -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
                -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
                -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
                -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
                -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
                -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
                -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
                -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
                -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
                -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
                -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
                -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
                -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
                -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
                -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
                -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
                -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
                -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
        };

    //定义拼音数组
    private static string[] getName = new string[]
        {
                "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
                "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
                "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
                "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
                "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
                "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
                "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
                "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
                "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
                "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
                "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
                "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
                "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
                "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
                "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
                "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
                "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
                "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
                "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
                "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
                "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
                "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
                "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
                "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
                "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
                "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
                "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
                "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
                "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
                "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
                "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
                "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
                "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
       };

    /// <summary>
    /// 汉字转全拼
    /// </summary>
    /// <returns></returns>
    public static string Convert2Pinyin(this string _s)
    {
        Regex reg = new Regex("^[\u4e00-\u9fa5]$");//验证是否输入汉字
        byte[] arr = new byte[2];
        string pystr = "";
        int asc = 0, M1 = 0, M2 = 0;
        char[] mChar = _s.ToCharArray();//获取汉字对应的字符数组
        for (int j = 0; j < mChar.Length; j++)
        {
            //如果输入的是汉字
            if (reg.IsMatch(mChar[j].ToString()))
            {
                arr = System.Text.Encoding.Default.GetBytes(mChar[j].ToString());
                M1 = (short)(arr[0]);
                M2 = (short)(arr[1]);
                asc = M1 * 256 + M2 - 65536;
                if (asc > 0 && asc < 160)
                {
                    pystr += mChar[j];
                }
                else
                {
                    switch (asc)
                    {
                        case -9254:
                            pystr += "Zhen"; break;
                        case -8985:
                            pystr += "Qian"; break;
                        case -5463:
                            pystr += "Jia"; break;
                        case -8274:
                            pystr += "Ge"; break;
                        case -5448:
                            pystr += "Ga"; break;
                        case -5447:
                            pystr += "La"; break;
                        case -4649:
                            pystr += "Chen"; break;
                        case -5436:
                            pystr += "Mao"; break;
                        case -5213:
                            pystr += "Mao"; break;
                        case -3597:
                            pystr += "Die"; break;
                        case -5659:
                            pystr += "Tian"; break;
                        default:
                            for (int i = (getValue.Length - 1); i >= 0; i--)
                            {
                                if (getValue[i] <= asc) //判断汉字的拼音区编码是否在指定范围内
                                {
                                    pystr += getName[i];//如果不超出范围则获取对应的拼音
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
            else//如果不是汉字
            {
                pystr += mChar[j].ToString();//如果不是汉字则返回
            }
        }
        return pystr;//返回获取到的汉字拼音
    }

    /// <summary>
    /// 汉字转首字母拼
    /// </summary>
    /// <returns></returns>
    public static string Convert2Py(this string _s)
    {
        int i = 0;
        ushort key = 0;
        string strResult = string.Empty;

        Encoding unicode = Encoding.Unicode;
        Encoding gbk = Encoding.GetEncoding(936);
        byte[] unicodeBytes = unicode.GetBytes(_s);
        byte[] gbkBytes = Encoding.Convert(unicode, gbk, unicodeBytes);
        while (i < gbkBytes.Length)
        {
            if (gbkBytes[i] <= 127)
            {
                strResult = strResult + (char)gbkBytes[i];
                i++;
            }
            #region 生成汉字拼音简码,取拼音首字母
            else
            {
                key = (ushort)(gbkBytes[i] * 256 + gbkBytes[i + 1]);
                if (key >= '\uB0A1' && key <= '\uB0C4')
                {
                    strResult = strResult + "A";
                }
                else if (key >= '\uB0C5' && key <= '\uB2C0')
                {
                    strResult = strResult + "B";
                }
                else if (key >= '\uB2C1' && key <= '\uB4ED')
                {
                    strResult = strResult + "C";
                }
                else if (key >= '\uB4EE' && key <= '\uB6E9')
                {
                    strResult = strResult + "D";
                }
                else if (key >= '\uB6EA' && key <= '\uB7A1')
                {
                    strResult = strResult + "E";
                }
                else if (key >= '\uB7A2' && key <= '\uB8C0')
                {
                    strResult = strResult + "F";
                }
                else if (key >= '\uB8C1' && key <= '\uB9FD')
                {
                    strResult = strResult + "G";
                }
                else if (key >= '\uB9FE' && key <= '\uBBF6')
                {
                    strResult = strResult + "H";
                }
                else if (key >= '\uBBF7' && key <= '\uBFA5')
                {
                    strResult = strResult + "J";
                }
                else if (key >= '\uBFA6' && key <= '\uC0AB')
                {
                    strResult = strResult + "K";
                }
                else if (key >= '\uC0AC' && key <= '\uC2E7')
                {
                    strResult = strResult + "L";
                }
                else if (key >= '\uC2E8' && key <= '\uC4C2')
                {
                    strResult = strResult + "M";
                }
                else if (key >= '\uC4C3' && key <= '\uC5B5')
                {
                    strResult = strResult + "N";
                }
                else if (key >= '\uC5B6' && key <= '\uC5BD')
                {
                    strResult = strResult + "O";
                }
                else if (key >= '\uC5BE' && key <= '\uC6D9')
                {
                    strResult = strResult + "P";
                }
                else if (key >= '\uC6DA' && key <= '\uC8BA')
                {
                    strResult = strResult + "Q";
                }
                else if (key >= '\uC8BB' && key <= '\uC8F5')
                {
                    strResult = strResult + "R";
                }
                else if (key >= '\uC8F6' && key <= '\uCBF9')
                {
                    strResult = strResult + "S";
                }
                else if (key >= '\uCBFA' && key <= '\uCDD9')
                {
                    strResult = strResult + "T";
                }
                else if (key >= '\uCDDA' && key <= '\uCEF3')
                {
                    strResult = strResult + "W";
                }
                else if (key >= '\uCEF4' && key <= '\uD188')
                {
                    strResult = strResult + "X";
                }
                else if (key >= '\uD1B9' && key <= '\uD4D0')
                {
                    strResult = strResult + "Y";
                }
                else if (key >= '\uD4D1' && key <= '\uD7F9')
                {
                    strResult = strResult + "Z";
                }
                else
                {
                    strResult = strResult + "?";
                }
                i = i + 2;
            }
            #endregion
        }
        return strResult;
    }

    #endregion

    #region 字符串数值 前+0

    public static string AutoLengthStr(this string _s, int _length)
    {
        string s = "";
        if (_s.Length >= _length)
        {
            s = _s.Length > _length ? _s.Substring(0, _length) : _s;
        }
        else
        {
            int num = _length - _s.Length;
            s = _s;
            for (int i = 0; i < num; i++)
            {
                s = s.Insert(0, "0");
            }
        }

        return s;
    }

    #endregion
}