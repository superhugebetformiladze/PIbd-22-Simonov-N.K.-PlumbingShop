﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumbingShopFileImplement.Models;
using PlumbingShopContracts.ViewModels;
using PlumbingShopContracts.StoragesContracts;
using PlumbingShopContracts.BindingModels;

namespace PlumbingShopFileImplement.Implements
{
    public class OrderStorage : IOrderStorage
    {
        private readonly FileDataListSingleton source;

        public OrderStorage()
        {
            source = FileDataListSingleton.GetInstance();
        }

        public List<OrderViewModel> GetFullList()
        {
            return source.Orders.Select(CreateModel).ToList();
        }

        public List<OrderViewModel> GetFilteredList(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return source.Orders
                 .Where(rec => (!model.DateFrom.HasValue && !model.DateTo.HasValue
                 && rec.DateCreate.Date == model.DateCreate.Date) ||
                 (model.DateFrom.HasValue && model.DateTo.HasValue && rec.DateCreate.Date >= model.DateFrom.Value.Date && rec.DateCreate.Date <= model.DateTo.Value.Date) ||
                 (model.ClientId.HasValue && rec.ClientId == model.ClientId))
                 .Select(CreateModel)
                 .ToList();
        }

        public OrderViewModel GetElement(OrderBindingModel model)
        {
            if (model == null) return null;
            var order = source.Orders.FirstOrDefault(rec => rec.SanitaryEngineeringId == model.SanitaryEngineeringId || rec.Id == model.Id);
            return order != null ? CreateModel(order) : null;
        }

        public void Insert(OrderBindingModel model)
        {
            int maxId = source.Orders.Count > 0 ? source.Orders.Max(rec => rec.Id) : 0;
            var element = new Order { Id = maxId + 1 };
            source.Orders.Add(CreateModel(model, element));
        }

        public void Update(OrderBindingModel model)
        {
            var element = source.Orders.FirstOrDefault(rec => rec.Id == model.Id);
            if (element == null) throw new Exception("Элемент не найден");
            CreateModel(model, element);
        }

        public void Delete(OrderBindingModel model)
        {
            Order element = source.Orders.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null) source.Orders.Remove(element);
            else throw new Exception("Элемент не найден");
        }

        private static Order CreateModel(OrderBindingModel model, Order order)
        {
            order.SanitaryEngineeringId = model.SanitaryEngineeringId;
            order.ClientId = (int)model.ClientId;
            order.Count = model.Count;
            order.Sum = model.Sum;
            order.Status = model.Status;
            order.DateCreate = model.DateCreate;
            order.DateImplement = model.DateImplement;
            return order;
        }

        private OrderViewModel CreateModel(Order order)
        {
            SanitaryEngineering sanitaryEngineering = source.SanitaryEngineerings.FirstOrDefault(x => x.Id == order.SanitaryEngineeringId);
            if (sanitaryEngineering == null)
            {
                throw new Exception("продукт не найден");
            }

            return new OrderViewModel
            {
                Id = (int)order.Id,
                SanitaryEngineeringId = order.SanitaryEngineeringId,
                SanitaryEngineeringName = sanitaryEngineering.SanitaryEngineeringName,
                ClientId = order.ClientId,
                ClientFIO = source.Clients.FirstOrDefault(rec => rec.Id == order.ClientId)?.ClientFIO,
                Count = order.Count,
                Sum = order.Sum,
                Status = order.Status.ToString(),
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement
            };
        }
    }
}
