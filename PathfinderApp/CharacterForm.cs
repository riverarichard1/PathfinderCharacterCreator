﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;


/*This Script handles basically everything for the Character Form
 * There's code in here that could definitely use improvement. 
 * Since this project is very early, efficiency isn't a big concern at the moment, functionality is.
 */

namespace PathfinderApp
{

    public partial class CharacterForm : MetroFramework.Forms.MetroForm
    {
        //Create new character Object
        Character character = new Character();
        //Create a list of detailed skills
        List<Skills_Detail> detailedSkills = new List<Skills_Detail>();
        //Create a list of detailed feats
        List<Feat_Detail> detailedFeats = new List<Feat_Detail>();

        //create a list of the skill panels
        List<MetroFramework.Controls.MetroPanel> list_skillsPanels = new List<MetroFramework.Controls.MetroPanel>();
        //create a list of the skill panels
        List<MetroFramework.Controls.MetroPanel> list_featsPanels = new List<MetroFramework.Controls.MetroPanel>();
        //Create dictionary of slow EPT
        Dictionary<string, string> slow = new Dictionary<string, string>
        {
            { "1", "3,000" },
            { "2", "7,500" },
            { "3", "14,000" },
            { "4", "23,000" },
            { "5", "35,000" },
            { "6", "53,000" },
            { "7", "77,000" },
            { "8", "115,000" },
            { "9", "160,000" },
            { "10", "235,000" },
            { "11", "330,000" },
            { "12", "475,000" },
            { "13", "665,000" },
            { "14", "955,000" },
            { "15", "1,350,000" },
            { "16", "1,900,000" },
            { "17", "2,700,000" },
            { "18", "3,850,000" },
            { "19", "5,350,000" },
            { "20", ">5,350,000" }
        };
        //Create dictionary of medium EPT
        Dictionary<string, string> medium = new Dictionary<string, string>
        {
            { "1", "2,000" },
            { "2", "5,000" },
            { "3", "9,000" },
            { "4", "15,000" },
            { "5", "23,000" },
            { "6", "35,000" },
            { "7", "51,000" },
            { "8", "75,000" },
            { "9", "105,000" },
            { "10", "155,000" },
            { "11", "220,000" },
            { "12", "315,000" },
            { "13", "445,000" },
            { "14", "635,000" },
            { "15", "890,000" },
            { "16", "1,300,000" },
            { "17", "1,800,000" },
            { "18", "2,550,000" },
            { "19", "3,600,000" },
            { "20", ">3,600,000" }
        };
        //Create dictionary of fast EPT
        Dictionary<string, string> fast = new Dictionary<string, string>
        {
            { "1", "1,300" },
            { "2", "3,300" },
            { "3", "6,000" },
            { "4", "10,000" },
            { "5", "15,000" },
            { "6", "23,000" },
            { "7", "34,000" },
            { "8", "50,000" },
            { "9", "71,000" },
            { "10", "105,000" },
            { "11", "145,000" },
            { "12", "210,000" },
            { "13", "295,000" },
            { "14", "425,000" },
            { "15", "600,000" },
            { "16", "850,000" },
            { "17", "1,200,000" },
            { "18", "1,700,000" },
            { "19", "2,400,000" },
            { "20", ">2,400,000" }
        };
        //Create list of everything that will be updated when str mod changes
        List<MetroFramework.Controls.MetroLabel> strModList = new List<MetroFramework.Controls.MetroLabel>();
        //Create list of everything that will be updated when dex mod changes
        List<MetroFramework.Controls.MetroLabel> dexModList = new List<MetroFramework.Controls.MetroLabel>();
        //Create list of everything that will be updated when con mod changes
        List<MetroFramework.Controls.MetroLabel> conModList = new List<MetroFramework.Controls.MetroLabel>();
        //Create list of everything that will be updated when int mod changes
        List<MetroFramework.Controls.MetroLabel> intModList = new List<MetroFramework.Controls.MetroLabel>();
        //Create list of everything that will be updated when wis mod changes
        List<MetroFramework.Controls.MetroLabel> wisModList = new List<MetroFramework.Controls.MetroLabel>();
        //Create list of everything that will be updated when cha mod changes
        List<MetroFramework.Controls.MetroLabel> chaModList = new List<MetroFramework.Controls.MetroLabel>();

        //clsResize _form_resize;

        public CharacterForm()
        {
            InitializeComponent();


            this.StyleManager = metroStyleManager1;
            this.StyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Character_TabControl.SelectedIndex = 0;

            //change the default selected values on the combo boxes
            Allignment_comboBox.SelectedIndex = 0;
            characterLevel_comboBox.SelectedIndex = 0;
            size_comboBox.SelectedIndex = 0;
            XP_point_total_comboBox.SelectedIndex = 0;
            gender_ComboBox.SelectedIndex = 0;

            //Initital calculation of next level value 
            CalculateNextLevel();
            //Free up any memory the template is taking up 
            SkillPanelTemplate.Dispose();
            /*Add stuff to mod lists*/
            dexModList.Add(ac_dexMod_textbox);
            dexModList.Add(initiative_dexModifier_textbox);
            dexModList.Add(reflex_abilityMod_textbox);
            conModList.Add(fortitude_abilityMod_textbox);
            wisModList.Add(will_abilityMod_textbox);
            //Start task 
            StartSkillsTask();
            //Start task
            StartFeatsTask();

            //_form_resize = new clsResize(this);
            //this.Load += _Load;
            //this.Resize += _Resize;

        }

        //#region shit needed for resizing 
        //private void _Load(object sender, EventArgs e)
        //{
        //    _form_resize._get_initial_size();
        //}

        //private void _Resize(object sender, EventArgs e)
        //{
        //    _form_resize._resize();
        //}
        //#endregion

        //Create a new character
        //Should probably save and then create the new form
        private void newCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            CharacterForm characterForm = new CharacterForm();
            characterForm.Show();
        }

        //add input to appropriate player info 
        //This is probably going to be changed later on
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            character.characterInfo.characterName = characterName_textBox.Text;
            character.characterInfo.alignment = Allignment_comboBox.Text;
            character.characterInfo.playerName = Player_textBox.Text;
            character.characterInfo.characterLevel = characterLevel_comboBox.Text;
            character.characterInfo.deity = Deity_textBox.Text;
            character.characterInfo.homeland = HomeLand__textBox.Text;
            character.characterInfo.race = Race_textBox.Text;
            character.characterInfo.size = size_comboBox.Text;
            character.characterInfo.gender = gender_ComboBox.SelectedText;
            character.characterInfo.age = Age_textBox.Text;
            character.characterInfo.height = Height_textBox.Text;
            character.characterInfo.weight = Weight_textBox.Text;
            character.characterInfo.hairColor = Hair_textBox.Text;
            character.characterInfo.eyeColor = Eyes_textBox.Text;
            character.characterInfo.className = class_textbox.Text;
            character.characterInfo.currentXp = CurXp_textbox.Text;
            character.characterInfo.nextLevelXp = nextLevel_textbox.Text;
            character.characterInfo.saveCharacterInfo();
        }


        #region CharacterInfo tab functions

        //Change level based off xp
        private void CurXp_textbox_TextChanged(object sender, EventArgs e)
        {
            int curXp_int = 0;
            int nextLevel_int = 0;
            Int32.TryParse(CurXp_textbox.Text.Replace(",", ""), out curXp_int);
            Int32.TryParse(nextLevel_textbox.Text.Replace(",", ""), out nextLevel_int);
            //Console.WriteLine("curxp = " + curXp_int);
            //Console.WriteLine("next level = " + nextLevel_int);
            if (curXp_int == nextLevel_int)
            {
                //Eventually you can put in a number so large that it'll just reset back to 0
                // and curXp_int == nextLevel_int will be true again which will break the program
                // if you're level 20
                try
                {
                    characterLevel_comboBox.SelectedIndex += 1;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Congrats, you exceeded the max amount of XP and tried to break my program.");
                }
                CalculateNextLevel();
            }
            //Recalculate character level while curXp is greater than next level XP
            else if (curXp_int > nextLevel_int)
            {
                int characterLevel = 0;

                while (curXp_int > nextLevel_int)
                {
                    Int32.TryParse(characterLevel_comboBox.Text, out characterLevel);
                    //Console.WriteLine("Character LeVEL " + characterLevel);
                    //Stop the loop if we are greater than or equal to level 20
                    if (characterLevel >= 20)
                    {
                        break;
                    }
                    //add 1 to the level
                    characterLevel_comboBox.SelectedIndex += 1;
                    //subtract 
                    curXp_int -= nextLevel_int;
                    //Recalculate
                    CalculateNextLevel();
                }
            }

        }

        #endregion

        #region Ability And Skills

        #region Events
        //Update everything that relates to dex
        private void dex_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            dex_abilitymodifier_textbox.Text = CalculateModifier(dex_abilityScore_textbox.Text);


            //change the appropriate text around or whatever
            //ac_dexMod_textbox.Text = dex_abilitymodifier_textbox.Text;
            //initiative_dexModifier_textbox.Text = dex_abilitymodifier_textbox.Text.Replace("+", "");
            //reflex_abilityMod_textbox.Text = dex_abilitymodifier_textbox.Text.Replace("+", "");

            CalculateCMD();
        }
        private void con_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            con_abilitymodifier_textbox.Text = CalculateModifier(con_abilityScore_textbox.Text);

            //fortitude_abilityMod_textbox.Text = con_abilityScore_textbox.Text.Replace("+", ""); ;

        }

        private void wis_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            wis_abilitymodifier_textbox.Text = CalculateModifier(wis_abilityScore_textbox.Text);
            //will_abilityMod_textbox.Text = wis_abilityScore_textbox.Text.Replace("+", "");

        }
        private void str_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            str_abilitymodifier_textbox.Text = CalculateModifier(str_abilityScore_textbox.Text);
            CalculateCMD();
            CalculateCMB();
        }

        private void int_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            int_abilitymodifier_textbox.Text = CalculateModifier(int_abilityScore_textbox.Text);
        }

        private void cha_abilityScore_textbox_TextChanged(object sender, EventArgs e)
        {
            //calculate ability mod
            cha_abilitymodifier_textbox.Text = CalculateModifier(cha_abilityScore_textbox.Text);
        }
        private void initiative_dexModifier_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateInitiative();
        }

        private void initiative_miscModifier_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateInitiative();
        }
        private void baseAttackBonus_amount_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateCMD();
            CalculateCMB();
        }
        //Show or hide mod panel
        private void mods_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mods_checkBox.Checked)
            {
                //acMods_Panel.Visible = true;
                acMods_Panel.BringToFront();
            }

            else
            {
                //acMods_Panel.Visible = false;
                acMods_Panel.SendToBack();
            }
        }

        private void savingThrow_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (savingThrow_checkbox.Checked)
            {
                savingThrows_panel.BringToFront();
            }

            else
            {
                savingThrows_panel.SendToBack();
            }
        }

        private void characterLevel_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateNextLevel();
        }

        private void XP_point_total_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateNextLevel();
        }

        //Highlight menu label on mouse enter 
        private void Menu_Dropdown_label_MouseEnter(object sender, EventArgs e)
        {
            Menu_Dropdown_label.BackColor = Color.LightGray;
        }

        //Change backcolor to normal color on menu label
        private void Menu_Dropdown_label_MouseLeave(object sender, EventArgs e)
        {
            Color newColor = new Color();
            newColor = Color.FromArgb(1111111);
            Menu_Dropdown_label.BackColor = newColor;
        }

        //Show context menu when menu label is clicked 
        private void Menu_Dropdown_label_MouseClick(object sender, MouseEventArgs e)
        {
            ctxMenu.Show(Menu_Dropdown_label, 0, Menu_Dropdown_label.Height);
        }

        private void Character_TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /*ABILITY MOD VALUES CHANGED*/
        private void str_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < strModList.Count; i++)
            {
                strModList[i].Text = str_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }

        private void dex_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dexModList.Count; i++)
            {
                dexModList[i].Text = dex_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }

        private void con_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < conModList.Count; i++)
            {
                conModList[i].Text = con_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }

        private void int_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < intModList.Count; i++)
            {
                intModList[i].Text = int_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }

        private void wis_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < wisModList.Count; i++)
            {
                wisModList[i].Text = wis_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }

        private void cha_abilitymodifier_textbox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chaModList.Count; i++)
            {
                chaModList[i].Text = cha_abilitymodifier_textbox.Text.Replace("+", "");
            }
        }


        #endregion

        #region CALCULATIONS
        //I'm sleepy so this code kinda sucks
        //but call this to convert speed to feet
        private string ConvertSpeedToFeet(string speed)
        {
            int convert = 0;
            int x = 0;
            //convert speed string to int
            Int32.TryParse(speed, out convert);
            x = convert / 5;
            return x.ToString();
        }

        //Calculates AC for TotalAc, flatFooted, and Touch
        private void CalculateAC()
        {
            int totalAC = 0;
            int flatFooted = 0;
            int touch = 0;
            /*Convert shit*/
            int armorBonus, shieldBonus, sizeMod, naturalArmor, deflectionMod, miscMod, dexmod = 0;
            Int32.TryParse(ac_armorBonus_textbox.Text, out armorBonus);
            Int32.TryParse(ac_shieldBonus_textbox.Text, out shieldBonus);
            Int32.TryParse(ac_sizeMod_textbox.Text, out sizeMod);
            Int32.TryParse(ac_NaturalArmor_textbox.Text, out naturalArmor);
            Int32.TryParse(ac_deflectionMod_textbox.Text, out deflectionMod);
            Int32.TryParse(ac_miscMod_textbox.Text, out miscMod);
            Int32.TryParse(ac_dexMod_textbox.Text, out dexmod);

            //Possibly not calcualted right?
            totalAC = 10 + armorBonus + shieldBonus + dexmod + sizeMod + naturalArmor + deflectionMod + miscMod;
            flatFooted = 10 + armorBonus;
            touch = 10 + dexmod + sizeMod + deflectionMod + miscMod;

            //Make the labels the correct values
            acAmount_lbl.Text = totalAC.ToString();
            flatFooted_amount_lbl.Text = flatFooted.ToString();
            touchAmount_lbl.Text = touch.ToString();
        }

        //Calculates fortitude saving throw total
        private void CalculateFortSaves()
        {
            int total = 0;
            int baseSave, abilityMod, magicMod, miscMod, tempMod = 0;
            Int32.TryParse(fortitude_baseSave_textbox.Text, out baseSave);
            Int32.TryParse(fortitude_abilityMod_textbox.Text, out abilityMod);
            Int32.TryParse(fortitude_magicMod_textbox.Text, out magicMod);
            Int32.TryParse(fortitude_miscMod_textbox.Text, out miscMod);
            Int32.TryParse(fortitude_tempMod_textbox.Text, out tempMod);
            total = baseSave + abilityMod + magicMod + miscMod + tempMod;
            fortitudeSave_total_textbox.Text = total.ToString();
        }
        //Calculates Reflex saving throw total
        private void CalculateReflexSaves()
        {
            int total = 0;
            int baseSave, abilityMod, magicMod, miscMod, tempMod = 0;
            Int32.TryParse(reflex_baseSave_textbox.Text, out baseSave);
            Int32.TryParse(reflex_abilityMod_textbox.Text, out abilityMod);
            Int32.TryParse(reflex_magicMod_textbox.Text, out magicMod);
            Int32.TryParse(reflex_miscMod_textbox.Text, out miscMod);
            Int32.TryParse(reflex_tempMod_textbox.Text, out tempMod);
            total = baseSave + abilityMod + magicMod + miscMod + tempMod;
            reflexSave_total_textbox.Text = total.ToString();
        }
        //Calculates Will saving throw total
        private void CalculateWillSaves()
        {
            int total = 0;
            int baseSave, abilityMod, magicMod, miscMod, tempMod = 0;
            Int32.TryParse(will_baseSave_textbox.Text, out baseSave);
            Int32.TryParse(will_abilityMod_textbox.Text, out abilityMod);
            Int32.TryParse(will_magicMod_textbox.Text, out magicMod);
            Int32.TryParse(will_miscMod_textbox.Text, out miscMod);
            Int32.TryParse(will_tempMod_textbox.Text, out tempMod);
            total = baseSave + abilityMod + magicMod + miscMod + tempMod;
            willSave_total_textbox.Text = total.ToString();
        }
        //Calculates the ability modifier value
        private string CalculateModifier(string abilityScore)
        {
            int rawScore = 0;
            int output = 0;
            Int32.TryParse(abilityScore, out rawScore);
            output = (rawScore - 10) / 2;
            if (output < 0)
            {
                return output.ToString();
            }
            else
                return "+" + output.ToString();
        }
        //Calculate initiative value 
        private void CalculateInitiative()
        {
            int total = 0;
            int dexMod, miscMod = 0;
            Int32.TryParse(initiative_dexModifier_textbox.Text, out dexMod);
            Int32.TryParse(initiative_miscModifier_textbox.Text, out miscMod);
            total = dexMod + miscMod;
            initiativeTotal_lbl.Text = total.ToString();
        }

        //Calculates CMD value

        private void CalculateCMD()
        {
            //CMD = Base Attack Bonus + strength mod + size mod + dexMod+base mod(10)
            int cmd = 0;
            int baseAttackBonus, strMod, sizeMod = 0, dexMod = 0;
            int baseMod = 10;
            Int32.TryParse(baseAttackBonus_amount_textbox.Text, out baseAttackBonus);
            Int32.TryParse(str_abilitymodifier_textbox.Text, out strMod);
            //Int32.TryParse(, out ); //Don't worry about size mod?
            Int32.TryParse(dex_abilitymodifier_textbox.Text, out dexMod);
            cmd = baseAttackBonus + strMod + sizeMod + dexMod + baseMod;

            CMD_amount_textbox.Text = cmd.ToString();
        }
        //Calculates CMB value
        private void CalculateCMB()
        {
            //CMB = BaseAttackBonus + strength + sizeMod
            int cmb = 0;
            int baseAttackBonus, strMod, sizeMod = 0;
            Int32.TryParse(baseAttackBonus_amount_textbox.Text, out baseAttackBonus);
            Int32.TryParse(str_abilitymodifier_textbox.Text, out strMod);
            //Int32.TryParse(, out ); //Don't worry about size mod?
            cmb = baseAttackBonus + strMod + sizeMod;

            CMB_amount_textbox.Text = cmb.ToString();
        }

        //Calculates the experience needed for the next level
        private void CalculateNextLevel()
        {

            string currentProgressionRate = XP_point_total_comboBox.Text;
            string currentLevel = characterLevel_comboBox.Text;
            string nextLevelXP = "";

            /*Might change this logic to switch statements?*/
            //If we are progressing slow 
            if (currentProgressionRate == "Slow")
            {
                //If slow dict contains current level
                if (slow.ContainsKey(currentLevel))
                {
                    nextLevelXP = slow[currentLevel];
                }
            }
            //If we are progressing medium
            else if (currentProgressionRate == "Medium")
            {
                //If slow dict contains current level
                if (medium.ContainsKey(currentLevel))
                {
                    //next level xp = value at currentLevel key
                    nextLevelXP = medium[currentLevel];
                }
            }

            //If we are progressing fast
            else if (currentProgressionRate == "Fast")
            {
                //If slow dict contains current level
                if (fast.ContainsKey(currentLevel))
                {
                    //next level xp = value at currentLevel key
                    nextLevelXP = fast[currentLevel];
                }
            }

            nextLevel_textbox.Text = nextLevelXP;
        }
        #endregion

        #region OTHER FUNCTIONS

        //Creates a single skill panel 
        public void CreateSkillPanel(Skill skill)
        {
            MetroFramework.Controls.MetroPanel skill_panel = new MetroFramework.Controls.MetroPanel();
            MetroFramework.Controls.MetroCheckBox classSkill_checkbox = new MetroFramework.Controls.MetroCheckBox();
            MetroFramework.Controls.MetroLabel skillName_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel totalBonus_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel equalSign1_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel abilityType_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel AbilityMod_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel PlusSign1_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel PlusSign2_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel ranks_label = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroLabel MiscMod_label = new MetroFramework.Controls.MetroLabel();

            // 
            // SkillPanelTemplate
            // 
            skill_panel.SuspendLayout();
            skill_panel.Controls.Add(MiscMod_label);
            skill_panel.Controls.Add(PlusSign2_label);
            skill_panel.Controls.Add(ranks_label);
            skill_panel.Controls.Add(PlusSign1_label);
            skill_panel.Controls.Add(AbilityMod_label);
            skill_panel.Controls.Add(abilityType_label);
            skill_panel.Controls.Add(equalSign1_label);
            skill_panel.Controls.Add(totalBonus_label);
            skill_panel.Controls.Add(skillName_label);
            skill_panel.Controls.Add(classSkill_checkbox);
            skill_panel.HorizontalScrollbarBarColor = true;
            skill_panel.HorizontalScrollbarHighlightOnWheel = false;
            skill_panel.HorizontalScrollbarSize = 10;
            skill_panel.Location = SkillPanelTemplate.Location;
            skill_panel.Name = "SkillPanelTemplate";
            skill_panel.Size = SkillPanelTemplate.Size;
            skill_panel.TabIndex = 2;
            skill_panel.VerticalScrollbarBarColor = true;
            skill_panel.VerticalScrollbarHighlightOnWheel = false;
            skill_panel.VerticalScrollbarSize = 10;
            skill_panel.Theme = MetroFramework.MetroThemeStyle.Dark;
            skill_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // 
            // checkBoxTemplate
            // 
            //classSkill_checkbox.AutoSize = true;
            classSkill_checkbox.AutoSize = checkBoxTemplate.AutoSize;
            classSkill_checkbox.Location = checkBoxTemplate.Location;
            classSkill_checkbox.Name = checkBoxTemplate.Name;
            classSkill_checkbox.Size = checkBoxTemplate.Size;
            classSkill_checkbox.TabIndex = checkBoxTemplate.TabIndex;
            classSkill_checkbox.Text = checkBoxTemplate.Text;
            classSkill_checkbox.UseSelectable = checkBoxTemplate.UseSelectable;
            classSkill_checkbox.Theme = MetroFramework.MetroThemeStyle.Dark;

            // 
            // SkillNameTemplate
            // 
            //skillName_label.AutoSize = true;
            skillName_label.BorderStyle = SkillNameTemplate.BorderStyle;
            skillName_label.Location = SkillNameTemplate.Location;
            skillName_label.Name = SkillNameTemplate.Name;
            skillName_label.Size = SkillNameTemplate.Size;
            skillName_label.TabIndex = SkillNameTemplate.TabIndex;
            skillName_label.Text = SkillNameTemplate.Text;
            skillName_label.TextAlign = SkillNameTemplate.TextAlign;

            skillName_label.Theme = MetroFramework.MetroThemeStyle.Dark;


            // 
            // TotalBonusTemplate
            // 
            totalBonus_label.AutoSize = TotalBonusTemplate.AutoSize;
            totalBonus_label.Location = TotalBonusTemplate.Location;
            totalBonus_label.Name = TotalBonusTemplate.Name;
            totalBonus_label.Size = TotalBonusTemplate.Size;
            totalBonus_label.TabIndex = TotalBonusTemplate.TabIndex;
            totalBonus_label.Text = TotalBonusTemplate.Text;
            totalBonus_label.BorderStyle = TotalBonusTemplate.BorderStyle;
            totalBonus_label.TextAlign = TotalBonusTemplate.TextAlign;

            totalBonus_label.Theme = MetroFramework.MetroThemeStyle.Dark;

            // 
            // EqualSignTemplate
            // 
            //equalSign1_label.AutoSize = true;
            equalSign1_label.AutoSize = EqualSignTemplate.AutoSize;
            equalSign1_label.Location = EqualSignTemplate.Location;
            equalSign1_label.Name = EqualSignTemplate.Name;
            equalSign1_label.Size = EqualSignTemplate.Size;
            equalSign1_label.TabIndex = EqualSignTemplate.TabIndex;
            equalSign1_label.Text = EqualSignTemplate.Text;

            equalSign1_label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // abilityTypeTemplate
            // 
            abilityType_label.AutoSize = abilityTypeTemplate.AutoSize;
            abilityType_label.Location = abilityTypeTemplate.Location;
            abilityType_label.Name = abilityTypeTemplate.Name;
            abilityType_label.Size = abilityTypeTemplate.Size;
            abilityType_label.TabIndex = abilityTypeTemplate.TabIndex;
            abilityType_label.Text = abilityTypeTemplate.Text;
            abilityType_label.TextAlign = abilityTypeTemplate.TextAlign;

            abilityType_label.Theme = MetroFramework.MetroThemeStyle.Dark;

            // 
            // AbilityModTemplate
            // 
            AbilityMod_label.AutoSize = AbilityModTemplate.AutoSize;
            AbilityMod_label.Location = AbilityModTemplate.Location;
            AbilityMod_label.Name = AbilityModTemplate.Name;
            AbilityMod_label.Size = AbilityModTemplate.Size;
            AbilityMod_label.TabIndex = AbilityModTemplate.TabIndex;
            AbilityMod_label.Text = AbilityModTemplate.Text;
            AbilityMod_label.BorderStyle = AbilityModTemplate.BorderStyle;
            AbilityMod_label.TextAlign = AbilityModTemplate.TextAlign;
            AbilityMod_label.Theme = MetroFramework.MetroThemeStyle.Dark;


            // 
            // PlusSignTemplate1
            // 
            PlusSign1_label.AutoSize = PlusSignTemplate1.AutoSize;
            PlusSign1_label.Location = PlusSignTemplate1.Location;
            PlusSign1_label.Name = PlusSignTemplate1.Name;
            PlusSign1_label.Size = PlusSignTemplate1.Size;
            PlusSign1_label.TabIndex = PlusSignTemplate1.TabIndex;
            PlusSign1_label.Text = PlusSignTemplate1.Text;

            PlusSign1_label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // PlusSignTemplate2
            // 
            PlusSign2_label.AutoSize = PlusSignTemplate2.AutoSize;
            PlusSign2_label.Location = PlusSignTemplate2.Location;
            PlusSign2_label.Name = PlusSignTemplate2.Name;
            PlusSign2_label.Size = PlusSignTemplate2.Size;
            PlusSign2_label.TabIndex = PlusSignTemplate2.TabIndex;
            PlusSign2_label.Text = PlusSignTemplate2.Text;

            PlusSign2_label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // ranksTemplate
            // 
            ranks_label.AutoSize = ranks_Template.AutoSize;
            ranks_label.BorderStyle = ranks_Template.BorderStyle;
            ranks_label.Location = ranks_Template.Location;
            ranks_label.Name = ranks_Template.Name;
            ranks_label.Size = ranks_Template.Size;
            ranks_label.TabIndex = ranks_Template.TabIndex;
            ranks_label.Text = ranks_Template.Text;
            ranks_label.TextAlign = ranks_Template.TextAlign;
            ranks_label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // MiscModTemplate
            // 
            MiscMod_label.AutoSize = miscMod_template.AutoSize;
            MiscMod_label.BorderStyle = miscMod_template.BorderStyle;
            MiscMod_label.Location = miscMod_template.Location;
            MiscMod_label.Name = miscMod_template.Name;
            MiscMod_label.Size = miscMod_template.Size;
            MiscMod_label.TabIndex = miscMod_template.TabIndex;
            MiscMod_label.Text = miscMod_template.Text;
            MiscMod_label.TextAlign = miscMod_template.TextAlign;

            MiscMod_label.Theme = MetroFramework.MetroThemeStyle.Dark;

            skill_panel.ResumeLayout(false);
            skill_panel.PerformLayout();
            this.Skills_TabPage.Controls.Add(skill_panel);
            // 
            // Set the info
            // 
            skillName_label.Text = skill.skillName;
            totalBonus_label.Text = skill.totalBonus;
            abilityType_label.Text = skill.modType;
            AbilityMod_label.Text = skill.abilityMod;
            ranks_label.Text = skill.ranks;
            MiscMod_label.Text = skill.miscMod;
            SetModType(abilityType_label, AbilityMod_label);


            //Add skill panel to list 
            list_skillsPanels.Add(skill_panel);

            //Add skill panel to skillsPage 
            SkillsPage_Panel.Controls.Add(skill_panel);
        }

        //Create all the skills panels using the information from the detailed skills list 
        public void CreateABunchOfSkills(List<Skills_Detail> aBunchOSkills)
        {
            foreach (Skills_Detail dSkill in aBunchOSkills)
            {
                Skill aSkill = new Skill(dSkill.name, "0", dSkill.stat, "0", "0", "0");
                CreateSkillPanel(aSkill);
                PlaceSkillPanels();
            }

        }

        //Places all of the skill panels
        public void PlaceSkillPanels()
        {
            //Don't place the first one 
            if (list_skillsPanels.Count > 1)
            {
                Point newLocation = new Point(list_skillsPanels[list_skillsPanels.Count - 2].Location.X, list_skillsPanels[list_skillsPanels.Count - 2].Location.Y + 41);
                list_skillsPanels[list_skillsPanels.Count - 1].Location = newLocation;
            }
        }

        //Returns a list of detailed skill data
        public List<Skills_Detail> GetSkillsData()
        {
            List<Skills_Detail> detailedList = new List<Skills_Detail>();
            //Create a list of ASkill objects from the string from the skills site
            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                //Get all skill names 
                var json = n.DownloadString("https://pathfinder-lookup.herokuapp.com/skills/all");
                string valueOriginal = Convert.ToString(json);
                //Create a list of all skill names
                List<ASkill> myDeserializedObjList = (List<ASkill>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<ASkill>));
                foreach (ASkill askill in myDeserializedObjList)
                {
                    //from a skill name, get the details for it 
                    json = n.DownloadString("https://pathfinder-lookup.herokuapp.com/skills/detail?name=" + askill.name);
                    //Console.WriteLine(json);
                    //Each single skill detail is held as a list, so I have to create a list to match that format
                    List<Skills_Detail> deserialSkill = (List<Skills_Detail>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<Skills_Detail>));
                    //Get first element in list and add it to the details list 
                    detailedList.Add(deserialSkill[0]);
                }
            }
            WriteJSONToFile(detailedList, "data", "DetailedSkills");
            //foreach (Skills_Detail detailedSkill in detailedList) {
            //    Console.WriteLine("Name: " +detailedSkill.name + "Stat: "+ detailedSkill.stat);
            //}
            return detailedList;
        }

        public void SetModType(MetroFramework.Controls.MetroLabel type, MetroFramework.Controls.MetroLabel mod)
        {
            //This is kind of a garbage way to do this, but whatever 
            if (type.Text.ToLower() == "str")
            {
                strModList.Add(mod);
            }
            else if (type.Text.ToLower() == "dex")
            {
                dexModList.Add(mod);
            }
            else if (type.Text.ToLower() == "con")
            {
                conModList.Add(mod);
            }
            else if (type.Text.ToLower() == "int")
            {
                intModList.Add(mod);
            }
            else if (type.Text.ToLower() == "wis")
            {
                wisModList.Add(mod);
            }
            else if (type.Text.ToLower() == "cha")
            {
                chaModList.Add(mod);
            }
        }

        //Task/thread calls this to get the skill data 
        public void GetSkillsDataThread()
        {
            //Get the skills data from file if it exists 
            string skillsLocation = "./data/DetailedSkills.json";
            if (File.Exists(skillsLocation))
            {
                // read file into a string and deserialize JSON to a type
                List<Skills_Detail> deserialSkill = JsonConvert.DeserializeObject<List<Skills_Detail>>(File.ReadAllText(skillsLocation));
                for (int i = 0; i < deserialSkill.Count; i++)
                {

                    detailedSkills.Add(deserialSkill[i]);
                }
                //print
                Console.WriteLine("DETAILED SKILL DATA LOADED FROM FILE");
            }
            //Or get from site 
            else
            {
                detailedSkills = GetSkillsData();
                //print
                Console.WriteLine("DETAILED SKILL DATA LOADED FROM API");
            }

            //Save skills to file
            //Enable the skills buttons
            SkillsPage_Panel.SafeInvoke(d => CreateABunchOfSkills(detailedSkills));
        }
        //Start a task/thread to get the skills data
        public void StartSkillsTask()
        {
            Task task = Task.Factory.StartNew(this.GetSkillsDataThread);
        }

        #endregion

        #region speed Text Change stuff

        private void baseSpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            baseSpeed_squares_lbl.Text = ConvertSpeedToFeet(baseSpeed_feet_textbox.Text);
        }

        private void armorSpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            armorSpeed_squares_lbl.Text = ConvertSpeedToFeet(armorSpeed_feet_textbox.Text);
        }

        private void flySpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            flySpeed_squares_lbl.Text = ConvertSpeedToFeet(flySpeed_feet_textbox.Text);
        }

        private void swimSpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            swimSpeed_squares_lbl.Text = ConvertSpeedToFeet(swimSpeed_feet_textbox.Text);
        }

        private void climbSpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            climbSpeed_squares_lbl.Text = ConvertSpeedToFeet(climbSpeed_feet_textbox.Text);
        }

        private void burrowSpeed_feet_textbox_TextChanged(object sender, EventArgs e)
        {
            burrowSpeed_squares_label.Text = ConvertSpeedToFeet(burrowSpeed_feet_textbox.Text);
        }

        #endregion

        #region AC text change stuff
        private void ac_armorBonus_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAC();
        }

        private void ac_shieldBonus_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAC();
        }

        private void ac_dexMod_textbox_TextChanged(object sender, EventArgs e)
        {
            //ac_dexMod_textbox.Text = dex_abilitymodifier_textbox.Text.Replace("+", "");
            CalculateAC();
        }

        private void ac_sizeMod_textbox_TextChanged(object sender, EventArgs e)
        {

            CalculateAC();
        }

        private void ac_NaturalArmor_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAC();
        }

        private void ac_deflectionMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAC();
        }

        private void ac_miscMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAC();
        }

        #endregion

        #region Saving throw text change stuff
        /*FORTITUDE*/
        private void fortitude_bseSave_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFortSaves();
        }

        private void fortitude_abilityMod_textbox_TextChanged(object sender, EventArgs e)
        {
            //fortitude_abilityMod_textbox.Text = con_abilitymodifier_textbox.Text.Replace("+", ""); ;
            CalculateFortSaves();
        }

        private void fortitude_magicMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFortSaves();
        }

        private void fortitude_miscMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFortSaves();
        }

        private void fortitude_tempMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFortSaves();
        }


        /*REFLEX*/
        private void reflex_baseSave_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateReflexSaves();
        }

        private void reflex_abilityMod_textbox_TextChanged(object sender, EventArgs e)
        {
            reflex_abilityMod_textbox.Text = dex_abilitymodifier_textbox.Text.Replace("+", "");
            CalculateReflexSaves();
        }

        private void reflex_magicMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateReflexSaves();
        }

        private void reflex_miscMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateReflexSaves();
        }

        private void reflex_tempMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateReflexSaves();
        }


        /*WISDOM*/
        private void will_baseSave_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateWillSaves();
        }

        private void will_abilityMod_textbox_TextChanged(object sender, EventArgs e)
        {
            //Remove the + sign 
            will_abilityMod_textbox.Text = wis_abilitymodifier_textbox.Text.Replace("+", "");
            CalculateWillSaves();
        }

        private void will_magicMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateWillSaves();
        }

        private void will_miscMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateWillSaves();
        }

        private void will_tempMod_textbox_TextChanged(object sender, EventArgs e)
        {
            CalculateWillSaves();
        }

        #endregion

        #endregion

        #region Feat Functions

        public void CreateFeatPanel(Feat feat)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterForm));
            MetroFramework.Controls.MetroPanel Feat_panel = new MetroFramework.Controls.MetroPanel();
            MetroFramework.Controls.MetroLabel Feat_Name_lbl = new MetroFramework.Controls.MetroLabel();
            MetroFramework.Controls.MetroButton Feat_showDescription_btn = new MetroFramework.Controls.MetroButton();




            // 
            // Feat_panel
            // 
            Feat_panel.BorderStyle = FeatTemplate_panel.BorderStyle;
            Feat_panel.Controls.Add(Feat_showDescription_btn);
            Feat_panel.Controls.Add(Feat_Name_lbl);
            Feat_panel.HorizontalScrollbarBarColor = FeatTemplate_panel.HorizontalScrollbarBarColor;
            Feat_panel.HorizontalScrollbarHighlightOnWheel = FeatTemplate_panel.HorizontalScrollbarHighlightOnWheel;
            Feat_panel.HorizontalScrollbarSize = FeatTemplate_panel.HorizontalScrollbarSize;
            Feat_panel.Location = FeatTemplate_panel.Location;
            Feat_panel.Name = "Feat_panel";
            Feat_panel.Size = FeatTemplate_panel.Size;
            Feat_panel.TabIndex = FeatTemplate_panel.TabIndex;
            Feat_panel.VerticalScrollbarBarColor = FeatTemplate_panel.VerticalScrollbarBarColor;
            Feat_panel.VerticalScrollbarHighlightOnWheel = FeatTemplate_panel.VerticalScrollbarHighlightOnWheel;
            Feat_panel.VerticalScrollbarSize = FeatTemplate_panel.VerticalScrollbarSize;
            Feat_panel.Controls.Add(Feat_Name_lbl);
            Feat_panel.Controls.Add(Feat_showDescription_btn);
            Feat_panel.Theme = MetroFramework.MetroThemeStyle.Dark;



            // 
            // FeatTemplate_Name_lbl
            // 
            Feat_Name_lbl.BorderStyle = FeatTemplate_Name_lbl.BorderStyle;
            Feat_Name_lbl.Location = FeatTemplate_Name_lbl.Location;
            Feat_Name_lbl.Name = "Feat_Name_lbl";
            Feat_Name_lbl.Size = FeatTemplate_Name_lbl.Size;
            Feat_Name_lbl.TabIndex = FeatTemplate_Name_lbl.TabIndex;
            Feat_Name_lbl.Text = "Feat Name";

            Feat_Name_lbl.Theme = MetroFramework.MetroThemeStyle.Dark;


            //
            // showDescription_btn
            //
            Feat_showDescription_btn.BackgroundImage = FeatTemplate_showDescription_btn.BackgroundImage;
            Feat_showDescription_btn.BackgroundImageLayout = FeatTemplate_showDescription_btn.BackgroundImageLayout;
            Feat_showDescription_btn.Location = FeatTemplate_showDescription_btn.Location;
            Feat_showDescription_btn.Name = "showDescription_btn";
            Feat_showDescription_btn.Size = FeatTemplate_showDescription_btn.Size;
            Feat_showDescription_btn.TabIndex = FeatTemplate_showDescription_btn.TabIndex;
            Feat_showDescription_btn.UseSelectable = FeatTemplate_showDescription_btn.UseSelectable;
            Feat_showDescription_btn.Click += (sender, EventArgs) => { FeatTemplate_showDescription_btn_Click(sender, EventArgs, feat.description); };
            Feat_Name_lbl.Text = feat.featName;
            Feat_Name_lbl.Theme = MetroFramework.MetroThemeStyle.Dark;


            Feat_panel.SuspendLayout();
            AllFeats_panel.Controls.Add(Feat_panel);
            Feat_panel.ResumeLayout(false);
            list_featsPanels.Add(Feat_panel);
        }

        //Create all the skills panels using the information from the detailed skills list 
        public void CreateABunchOfFeats(List<Feat_Detail> aBunchOfFeats)
        {
            foreach (Feat_Detail dFeat in aBunchOfFeats)
            {
                Feat aFeat = new Feat(dFeat.name, dFeat.description);
                CreateFeatPanel(aFeat);
                PlaceFeatPanels();
            }
        }

        public void AddFeatInfoToComboBox(List<Feat_Detail> aBunchOfFeats)
        {
            foreach (Feat_Detail dFeat in aBunchOfFeats)
            {
                Feat aFeat = new Feat(dFeat.name, dFeat.description);
                AllFeats_ComboBox.Items.Add(aFeat.featName);
            }

        }

        //Places all of the Feat panels
        public void PlaceFeatPanels()
        {
            //Don't place the first one 
            if (list_featsPanels.Count > 1)
            {
                Point newLocation = new Point(list_featsPanels[list_featsPanels.Count - 2].Location.X, list_featsPanels[list_featsPanels.Count - 2].Location.Y + 41);
                list_featsPanels[list_featsPanels.Count - 1].Location = newLocation;
            }
        }



        //Start a task/thread to get the feat data
        public void StartFeatsTask()
        {
            Task task = Task.Factory.StartNew(this.GetFeatDataThread);
        }

        //Task/thread calls this to get the feat data 
        public void GetFeatDataThread()
        {
            //Turn off add button 
            addNewFeat_Btn.SafeInvoke(d => addNewFeat_Btn.Enabled = false);
            //Get the skills data from file if it exists 
            string skillsLocation = "./data/DetailedFeats.json";
            if (File.Exists(skillsLocation))
            {
                // read file into a string and deserialize JSON to a type
                List<Feat_Detail> deserialFeat = JsonConvert.DeserializeObject<List<Feat_Detail>>(File.ReadAllText(skillsLocation));
                for (int i = 0; i < deserialFeat.Count; i++)
                {

                    detailedFeats.Add(deserialFeat[i]);
                }
                //print
                Console.WriteLine("DETAILED FEAT DATA LOADED FROM FILE");
            }
            //Or get from site 
            else
            {
                detailedFeats = GetFeatData();
                Console.WriteLine("DETAILED FEAT DATA LOADED FROM API");
            }

            //Turn on add button 
            addNewFeat_Btn.SafeInvoke(d => addNewFeat_Btn.Enabled = true);
            //Add all feats to the comboBox
            Feats_TabPage.SafeInvoke(d => AddFeatInfoToComboBox(detailedFeats));
        }

        //Returns a list of detailed feat data
        public List<Feat_Detail> GetFeatData()
        {
            List<Feat_Detail> detailedList = new List<Feat_Detail>();
            //Create a list of ASkill objects from the string from the skills site
            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                //Get all skill names 
                var json = n.DownloadString("https://pathfinder-lookup.herokuapp.com/feats/all");
                string valueOriginal = Convert.ToString(json);
                //Create a list of all skill names
                List<AFeat> myDeserializedObjList = (List<AFeat>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<AFeat>));
                foreach (AFeat afeat in myDeserializedObjList)
                {
                    //from a skill name, get the details for it 
                    json = n.DownloadString("https://pathfinder-lookup.herokuapp.com/feats/detail?name=" + afeat.name);
                    //Console.WriteLine(json);
                    //Each single skill detail is held as a list, so I have to create a list to match that format
                    List<Feat_Detail> deserialFeat = (List<Feat_Detail>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<Feat_Detail>));
                    //Get first element in list and add it to the details list 
                    detailedList.Add(deserialFeat[0]);
                }
            }
            WriteJSONToFile(detailedList, "data", "DetailedFeats");
            //foreach (Skills_Detail detailedSkill in detailedList) {
            //    Console.WriteLine("Name: " +detailedSkill.name + "Stat: "+ detailedSkill.stat);
            //}
            return detailedList;
        }




        #endregion

        /*
         * Method that accepts generic objects, the directory you're putting it in, and the file name. 
         * And then takes everything from that object and writes it to the location as a json file 
         */
        private void WriteJSONToFile<T>(T genObject, string directory, string fileName)
        {
            //Create a directory called Characters if it doesn't exist
            System.IO.Directory.CreateDirectory("./" + directory);
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(@directory + "/" + fileName + ".json", JsonConvert.SerializeObject(genObject));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(@directory + "/" + fileName + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, genObject);
            }
        }

        private void CloseFeatDescription_btn_Click(object sender, EventArgs e)
        {
            FeatDescription_panel.SendToBack();
            //AllFeats_panel.BringToFront();
            FeatDescription_panel.Hide();
        }

        private void FeatTemplate_showDescription_btn_Click(object sender, EventArgs e, String description)
        {
            //AllFeats_panel.SendToBack();
            FeatDescription_panel.BringToFront();
            FeatDescription_panel.Show();
            FeatDescription_textBox.Text = description;
        }

        private void addNewFeat_Btn_Click(object sender, EventArgs e)
        {
            AllFeats_ComboBox.Show();
        }

        private void AllFeats_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Feat newFeat = new Feat(detailedFeats[AllFeats_ComboBox.SelectedIndex].name, detailedFeats[AllFeats_ComboBox.SelectedIndex].description);
            CreateFeatPanel(newFeat);
            PlaceFeatPanels();
            AllFeats_ComboBox.Hide();
        }
    }
}

