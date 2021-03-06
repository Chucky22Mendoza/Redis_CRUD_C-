﻿using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RedisCRUD
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Edit(bool value)
        {
            txtID.ReadOnly = value;
            txtManufacturer.ReadOnly = value;
            txtModel.ReadOnly = value;
        }

        void ClearText()
        {
            txtID.Text = string.Empty;
            txtManufacturer.Text = string.Empty;
            txtModel.Text = string.Empty;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearText();
            phoneBindingSource.Add(new Phone());
            phoneBindingSource.MoveLast();
            Edit(false); //Allow edit
            txtID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Edit(false); //Allow edit
            txtID.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Edit(true); //Read only
            phoneBindingSource.ResetBindings(false);
            ClearText();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MetroFramework.MetroMessageBox.Show(this, "Are you sure want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Phone p = phoneBindingSource.Current as Phone;

                if (p != null)
                {
                    using (RedisClient client = new RedisClient("localhost", 6379))
                    {
                        IRedisTypedClient<Phone> phone = client.As<Phone>();
                        phone.DeleteById(p.ID);
                        phoneBindingSource.RemoveCurrent();
                        ClearText();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                phoneBindingSource.EndEdit();
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                phone.StoreAll(phoneBindingSource.DataSource as List<Phone>);
                MetroFramework.MetroMessageBox.Show(this, "Your data has been succesfully saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //ClearText();
                Edit(true); //Read Only
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                phoneBindingSource.DataSource = phone.GetAll();
                Edit(true); //Read only
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                phone.DeleteAll();
                phoneBindingSource.Clear();

                String new_id = "J_0";
                String new_manufacturer = "010-Enterprise-";
                String new_Model = "902-XXJ-";

                for(int i = 0; i<3000; i++)
                {
                    Phone p = new Phone();
                    p.ID = new_id + i;
                    p.Manufacturer = new_manufacturer + i;
                    p.Model = new_Model + i;

                    phone.Store(p);
                }

                phoneBindingSource.DataSource = phone.GetAll();
                MetroFramework.MetroMessageBox.Show(this, "Your data has been succesfully reseted.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearText();
            }
        }
    }
}
