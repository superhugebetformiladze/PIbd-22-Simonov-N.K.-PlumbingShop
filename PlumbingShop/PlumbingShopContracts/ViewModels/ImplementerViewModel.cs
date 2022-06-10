﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumbingShopContracts.Attributes;

namespace PlumbingShopContracts.ViewModels
{
    public class ImplementerViewModel
    {
        [Column(title: "Номер", width: 100, visible: false)]
        public int Id { get; set; }
        [Column(title: "ФИО исполнителя", gridViewAutoSize: GridViewAutoSize.Fill)]
        public string ImplementerFIO { get; set; }
        [Column(title: "Время работы", width: 100)]
        public int WorkingTime { get; set; }
        [Column(title: "Время отдыха", width: 100)]
        public int PauseTime { get; set; }
    }
}
