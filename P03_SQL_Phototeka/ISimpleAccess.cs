using System.Collections.Generic;
using System.Xml.Linq;

namespace P03_SQL_Phototeka
{
    interface ISimpleAccess
    {
        /// <summary>
        /// Приготовление к загрузке, выполняется, когда явно заказана загрзка
        /// </summary>
        void PrepareToLoad();
        /// <summary>
        /// Загрузка потока элементов. Возможен запуск нескольких потоков
        /// </summary>
        /// <param name="element_flow"></param>
        void LoadElementFlow(IEnumerable<XElement> element_flow);
        /// <summary>
        /// Явный запуск вычисления индексов базы данных
        /// </summary>
        void MakeIndexes();
        /// <summary>
        /// Посчет числа элементов в указанной таблице
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        long Count(string table);
        /// <summary>
        /// Делается выборка из таблицы записи (части записи) по идентификатору 
        /// </summary>
        /// <param name="id">идентификатор записи</param>
        /// <param name="table">имя таблицы</param>
        /// <returns></returns>
        object[] GetById(int id, string table);
        /// <summary>
        /// По заданному имени выдаются из таблицы записи, с "похожими" значениями поля name
        /// </summary>
        /// <param name="searchstring">начальная часть имени в любой смеси регистров</param>
        /// <param name="table">таблица поиска</param>
        /// <returns></returns>
        IEnumerable<object[]> SearchByName(string searchstring, string table);
        /// <summary>
        /// По таблице Reflection выявляются идентификаторы фоток, на которых изображен (отражен) персонаж с 
        /// идентификатором id, а затем организуется поток записей фотографий по этим идентификаторам.
        /// </summary>
        /// <param name="id">идентификатор персоны</param>
        /// <returns></returns>
        IEnumerable<object[]> GetPhotosOfPersonUsingRelation(int id);
    }
}
