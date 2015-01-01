using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace Yvonne2
{
    public class Pic
    {
        private string _thumb;
        private string _full;

        public string Thumb { get { return _thumb; } }
        public string Full { get { return _full; } }

        public Pic(string f, string t)
        {
            _full = f;
            _thumb = t;
        }
    }

    public class Yvonne : IEnumerable
    {
        private Pic[] _pics;

        public bool Ready;

        public Yvonne(Pic[] picList)
        {
            _pics = new Pic[picList.Length];

            for (int c = 0; c < picList.Length; c++)
            {
                _pics[c] = picList[c];
            }
        }

        public Yvonne(string url)
        {
            try
            {
                XmlNodeList xl;
                XmlDocument doc = new XmlDocument();
                doc.Load(url);
                xl = doc.SelectNodes("//image");
                _pics = new Pic[xl.Count];
                int c = 0;

                foreach (XmlNode xn in xl)
                {
                    string full = xn.FirstChild.InnerText;
                    string thumb = xn.LastChild.InnerText;
                    _pics[c] = new Pic(full, thumb);
                    c++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem loading XML", ex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public YvEnum GetEnumerator()
        {
            return new YvEnum(_pics);
        }
    }

    public class YvEnum : IEnumerator
    {
        private Pic[] _pics;

        int position = -1;

        public YvEnum(Pic[] picList)
        {
            _pics = picList;
        }

        public bool MoveNext()
        {
            //DateTime dt = ;
            int seed = int.Parse(DateTime.Now.ToString("ssmmHH"));
            Random r = new Random(seed);
            position = r.Next(1, _pics.Length) - 1;
            return true;
        }

        public void Reset() { }

        object IEnumerator.Current { get { return Current; } }

        public Pic Current
        {
            get
            {
                return _pics[position];
            }
        }
    }
}
