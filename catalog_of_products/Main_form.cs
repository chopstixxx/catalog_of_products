using Npgsql;
using System.Data;

namespace catalog_of_products
{
    public partial class Main_form : Form
    {
        readonly string conn_string = "Server=localhost;Port=5432;Database=catalog_bd;User Id=postgres;Password=123;";
        private readonly List<ListViewItem> allItems = new List<ListViewItem>();
        private List<ListViewItem> displayedItems = new List<ListViewItem>();

        private int itemsPerPage = 5;
        private int currentPage = 0;

        public Main_form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Дороже");
            comboBox1.Items.Add("Дешевле");
            comboBox1.Items.Add("По алфавиту (А-Я)");
            comboBox1.Items.Add("По алфавиту (Я-А)");


            comboBox2.Items.Add("Все типы");
            comboBox2.Items.Add("Мебель");
            comboBox2.Items.Add("Бытовая техника");
            comboBox2.Items.Add("Посуда");


            Load_data();
            displayedItems = allItems;
            DisplayCurrentPage(); // Отображаем первую страницу после загрузки данных

        }


        private void Load_data()
        {
            listView1.Items.Clear();

            ImageList imageList = new ImageList();

            imageList.ImageSize = new Size(100, 100);
            imageList.Images.Add(new Bitmap("images/product.png"));


            listView1.SmallImageList = imageList;

            using (NpgsqlConnection conn = new NpgsqlConnection(conn_string))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM goods", conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long article = reader.GetInt64(reader.GetOrdinal("article"));
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            int price = reader.GetInt32(reader.GetOrdinal("price"));


                            ListViewItem listViewItem = new ListViewItem(new string[] { "", Convert.ToString(article), name, string.Format("{0}₽", price) });
                            listViewItem.ImageIndex = 0;
                            listViewItem.Tag = reader.GetInt32(reader.GetOrdinal("type_id"));
                            allItems.Add(listViewItem); // Добавляем элемент в список всех элементов
                            listView1.Items.Add(listViewItem);
                        }
                    }
                }
            }




        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Обновляем отображаемые элементы в соответствии с введенным текстом
            string searchText = textBox1.Text.ToLower();
            displayedItems = allItems.Where(item => item.SubItems[2].Text.ToLower().StartsWith(searchText)).ToList();

            DisplayCurrentPage();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int nextPage = currentPage + 1;
            int startIndex = nextPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, displayedItems.Count);

            if (endIndex > startIndex)
            {
                currentPage++;
                DisplayCurrentPage();
            }
        }

        private void DisplayCurrentPage()
        {
            listView1.Items.Clear();

            // Вычисляем индекс начала и конца элементов для текущей страницы
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, displayedItems.Count);

            // Добавляем элементы текущей страницы в ListView
            for (int i = startIndex; i < endIndex; i++)
            {
                listView1.Items.Add(displayedItems[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Переход к предыдущей странице
            if (currentPage > 0)
            {
                currentPage--;
                DisplayCurrentPage();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    displayedItems.Sort((item1, item2) => int.Parse(item2.SubItems[3].Text.Replace("₽", "")).CompareTo(int.Parse(item1.SubItems[3].Text.Replace("₽", ""))));
                    break;
                case 1:
                    displayedItems.Sort((item1, item2) => int.Parse(item1.SubItems[3].Text.Replace("₽", "")).CompareTo(int.Parse(item2.SubItems[3].Text.Replace("₽", ""))));
                    break;
                case 2:
                    displayedItems.Sort((item1, item2) => item1.SubItems[2].Text.CompareTo(item2.SubItems[2].Text));
                    break;
                case 3:
                    displayedItems.Sort((item1, item2) => item2.SubItems[2].Text.CompareTo(item1.SubItems[2].Text));
                    break;
            }

            DisplayCurrentPage();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обновляем отображаемые элементы в соответствии с выбранным типом
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    displayedItems = allItems;
                    break;
                case 1:
                    displayedItems = allItems.Where(item => (int)item.Tag == 1).ToList();
                    break;
                case 2:
                    displayedItems = allItems.Where(item => (int)item.Tag == 2).ToList();
                    break;
                case 3:
                    displayedItems = allItems.Where(item => (int)item.Tag == 3).ToList();
                    break;
            }

            DisplayCurrentPage();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Получаем выбранный элемент ListView
            ListViewItem selectedItem = listView1.SelectedItems[0];

            // Получаем артикул из выбранного элемента
            string article = selectedItem.SubItems[1].Text;
            Article_form newForm = new Article_form(article);
            newForm.Show();

        }
    }
}