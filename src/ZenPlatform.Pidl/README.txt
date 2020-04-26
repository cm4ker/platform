PIDL  (Platform Interface Definition Language)
Язык для описания интерфейса пользователя для платформы

Было столкновениие с тем, что у нас есть, допустим следующая структура
<form xmlns:table="TableExtension" 
	  DataType="Document.ПриходнаяНакладная">
    
    <fragment Id="1" DataType="Document.ПриходнаяНакладная">
        Здесь описание компонента
    </fragment>

     <group>
 
        <field Source="Комментарий" Height="1" Width="200" />
	
		<field Source="Текст1" AdvancedMode="true" Height="25" Width="200" Margin="0,10,23,10" Padding="10,10,10,10" />
        
        <field Source="Товары">
            <table:extension>
                <columns>
                    <column Source="Колонка1" />
                    <column Source="Колонка2" />
                    <column Source="Колонка3" />
                </columns>
            </table:extension>
        </field>
		        
        <field Source="ДатаДокумента" Enable="true" />

    </group>

    <fragment_ref Id="1" /> 
	<fragment_ref Id="1" /> 
</form>

У нас есть такая вот с виду непримечательная директива, которая регистрируется динамически, с подключаемым компонентом