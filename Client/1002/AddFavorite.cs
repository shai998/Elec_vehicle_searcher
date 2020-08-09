using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1002
{
    public partial class AddFavorite : Form
    {
        public string Addr { get; private set; }
        private List<Favorite> favlist = null;
        
        public AddFavorite(List<Favorite> f_list)
        {

            InitializeComponent();
            favlist = new List<Favorite>();
            favlist = f_list;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddFavorite_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.BeginUpdate();

            listView1.Columns.Add("주소",200);
            listView1.Columns.Add("이름",200);

            for (int i = 0; i < favlist.Count; i++)
            {
                ListViewItem item = new ListViewItem(favlist[i].Address);
                item.SubItems.Add(favlist[i].Name);

                listView1.Items.Add(item);
            }

            listView1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Addr = listView1.FocusedItem.SubItems[0].Text;
            Name = listView1.FocusedItem.SubItems[1].Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
