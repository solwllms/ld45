using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ld45
{
    class s_save
    {
        public static void SaveGame()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog1.Filter = "Population save game|*.cty";
            saveFileDialog1.Title = "Save game";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                using (BinaryWriter bin = new BinaryWriter(fs))
                {
                    bin.Write(DateTime.Now.ToString());
                    bin.Write("untitled");

                    bin.Write(g_game.datetime.ToString());
                    bin.Write(g_game.money);
                    bin.Write(g_game.houseTax);

                    bin.Write(g_game.cameraX);
                    bin.Write(g_game.cameraY);

                    bin.Write(g_map.size);
                    for (int x = 0; x < g_map.size; x++)
                    {
                        for (int y = 0; y < g_map.size; y++)
                        {
                            bin.Write(g_map.terrain[x, y]);
                            bin.Write(g_map.water[x, y]);
                            bin.Write(g_map.buildings[x, y]);
                        }
                    }
                }

                fs.Close();
            }
        }

        public static void LoadGame()
        {
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog1.Filter = "Population save game|*.cty";
            saveFileDialog1.Title = "Load game";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                try
                {
                    using (BinaryReader bin = new BinaryReader(fs))
                    {
                        DateTime save = DateTime.Parse(bin.ReadString());
                        string name = bin.ReadString();

                        g_game.datetime = DateTime.Parse(bin.ReadString());
                        g_game.money = bin.ReadInt32();
                        g_game.houseTax = bin.ReadInt32();

                        g_game.cameraX = bin.ReadInt32();
                        g_game.cameraY = bin.ReadInt32();

                        g_map.size = bin.ReadInt32();
                        for (int x = 0; x < g_map.size; x++)
                        {
                            for (int y = 0; y < g_map.size; y++)
                            {
                                g_map.terrain[x, y] = bin.ReadInt32();
                                g_map.water[x, y] = bin.ReadBoolean();
                                g_map.buildings[x, y] = bin.ReadInt32();
                            }
                        }

                        g_map.Init();
                        s_menu.CloseMenu();
                    }
                }
                catch
                {
                    MessageBox.Show("The save may be corrupt or not work in this version, sorry about that.", "Failed to load!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    g_map.Generate(50);
                    s_menu.CloseMenu();
                }

                fs.Close();
            }
        }
    }
}
