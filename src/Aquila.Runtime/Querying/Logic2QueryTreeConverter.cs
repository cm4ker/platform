using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying
{
    public class Logic2QueryTreeConverter
    {
        private QueryMachine _qm;

        public Logic2QueryTreeConverter()
        {
            _qm = new QueryMachine();
        }

        public object Convert(QSelectQuery query)
        {
            GenerateQuery(query);
            return _qm.pop();
        }

        private void GenerateQuery(QSelectQuery q)
        {
            _qm.bg_query();

            _qm.m_from();
            GenerateFrom(q.From);

            _qm.m_select();
            GenerateSelect(q.Select);
            _qm.st_query();
        }

        private void GenerateSource(QObject ot)
        {
            /*
             FROM
                    Invoice i
                    LEFT JOIN Store s ON i.Store = s.Id AND i.TypeId = 2
                    LEFT JOIN Department d ON i.Store = d.Id AND i.TypeId = 3
                    LEFT JOIN Custom c ON i.Store = c.Id AND i.TypeId = 4
                WHERE
                    i.Id = @Id
                SELECT
                    COALESE(s.Name, d.Name, c.Name) as Name
                    
                    
                    =>
                    
                    
                SELECT
                    
                FROM
                    _Obje AS Expr1 --Invoice i
                    LEFT JOIN  (SELECT A,B,C  FROM _Obje123) AS Expr2 ON Expr1.A = Expr2                       --
                    
                    
                From(x=> x.TableName("A"))
                
                From(x=> x.FromRaw("Select * From Test"))
             */

            throw new NotImplementedException();
        }

        private void GenerateFrom(QFrom from)
        {
            GenerateDataSource(from.Source);

            //TODO: делаем join
        }

        private void GenerateDataSource(QDataSource ds)
        {
            if (ds is QNestedQuery nq)
            {
                GenerateQuery(nq.Nested);
            }
            else if (ds is QObject ot)
            {
                GenerateSource(ot);
            }
            else if (ds is QAliasedDataSource ads)
            {
                GenerateDataSource(ads.ParentSource);
                _qm.@as(ads.Alias);
            }
        }

        private void GenerateSelect(QSelect s)
        {
            foreach (var field in s.Fields)
            {
                GenerateField(field);
            }
        }

        private void GenerateExpression(QExpression q)
        {
        }

        private void GenerateItem(QLangElement item)
        {
            switch (item)
            {
                case QField f:
                    GenerateField(f);
                    break;
            }
        }

        private void GenerateExpression()
        {
        }

        private void GenerateField(QField field)
        {
            if (field is QSourceFieldExpression sf)
            {
                GenerateSourceField(sf);
            }
            else if (field is QAliasedSelectExpression ase)
            {
                if (ase.GetChildren().First() is QSourceFieldExpression f)
                {
                    GenerateSourceField(f, ase.Alias);
                }
                else
                {
                }
            }
            else if (field is QIntermediateSourceField isf)
            {
                if (isf.DataSource is QAliasedDataSource qads)
                {
                    _qm.ld_str(qads.Alias);
                }
                else
                {
                    _qm.ld_str(null);
                }
            }
        }

        private void GenerateFieldSchema(object def, string alias = null)
        {
            throw new NotImplementedException();
            // _qm.ld_str(def.FullName);
            // _qm.ld_column();
            //
            // if (alias != null)
            //     _qm.@as(def.Prefix + alias + def.Postfix);
        }

        private void GenerateSourceField(QSourceFieldExpression sf, string alias = null)
        {
            throw new NotImplementedException();
            // List<ColumnSchemaDefinition> schema = null; //sf.Property.GetPropertySchemas();
            //
            // foreach (var def in schema)
            // {
            //     GenerateFieldSchema(def, alias);
            // }
        }
    }
}