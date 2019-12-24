using System;

namespace ZenPlatform.Compiler.Contracts
{
    /*
     Ссылка.
     
     Необходима для того, чтобы можно было лишь сослаться на объект загружая его не полностью
     
     (Id) (Ref)
      
     Для каждого компонента, который представляет единичный объект генерируется следующее
     
     LinkClass
     {
        private string _description 
     
        public LinkClass(int typeId, Guid id, Dctionary[string, string] bag)
        {
            if(typeId == 10) _description = $"Invoice from {bag["Date"]}";
            
            ....
            
            TypeId = typeId;
            Id = id;
        }
        
        
        public string Description {get;}
        public int TypeId {get;}
        public Guid Id {get;}
     } 
         
   Прмер того, как будет мапиться запрос
     {
        while(DataReader.Read())
        {
            Мы знаем, кто сюда может прийти, какие поля и так далее
    
            (Documents, References)
                if(DataReader[_Type1] == 5)
                {
                     var documentBag =  {construct some document bag from DataReader}
                     var link = new DocumentLink(DataReader[_Type1], DataReader[_Id], documentBag)
                }
                else if (DataReader[_Type1] == 6)
                {
                    ....                        
                }
                else if .....
        }
     
     }     
          
     
     */
    /// <summary>
    /// Ссылка на единичный объект компонента 
    /// </summary>
    public interface ILink
    {
        Guid Id { get; }
        int Type { get; }

        string Presentation { get; }
    }
}