namespace DotvvmBlog.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FillData : DbMigration
    {
        public override void Up()
        {
            Sql(@"SET IDENTITY_INSERT [dbo].[Blogs] ON 

INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (3, N'Vývojářský blog Tomáše Hercega', N'', N'Tomáš Herceg')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (4, N'Vývojářský blog Tomáše Jechy', N'', N'Tomáš Jecha')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (7, N'Programy a aplikace od našich čtenářů', N'', N'Od našich čtenářů')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (15, N'Vývojářský blog Jana Holana a Tomáše Holana', N'', N'Null Reference Exception')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (16, N'Vývojářský blog Martina Dybala', N'', N'Martin Dybal')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (18, N'Vývojářský blog - Daniel Vittek', N'', N'Daniel Vittek')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (19, N'Vývojářský blog - Martin Vodák', N'', N'Martin Vodák')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (22, N'Vývojářský blog Milana Mikuše', N'', N'Milan Mikuš')
INSERT [dbo].[Blogs] ([Id], [Title], [Subtitle], [AuthorName]) VALUES (24, N'Vývojářský blog Filipa Herudka', N'', N'Filip Herudek')
SET IDENTITY_INSERT [dbo].[Blogs] OFF
SET IDENTITY_INSERT [dbo].[Articles] ON 

INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (6393, 16, CAST(N'2015-10-24T00:00:00.000' AS DateTime), N'Aspektově orientované programování - Úvod', N'<p>Už jste někdy viděli metodu, která vypadá nějak takto?</p><pre class=""brush: csharp"">public IEnumerable&lt;IGrouping&lt;TKey, T&gt;&gt; GroupWhile(
    IEnumerable&lt;T&gt; source,
    Func&lt;T, T, bool&gt; predicate,
    Func&lt;T, T, TKey&gt; keyOfGroup)
{
    Log.Debug(""GroupWhile has started"");
    if (source == null)
    {
        throw new ArgumentNullException(""source"");
    }
    if (predicate == null)
    {
        throw new ArgumentNullException(""predicate"");
    }
    if (keyOfGroup == null)
    {
        throw new ArgumentNullException(""keyOfGroup"");
    }
    try
    {
        var enumerator = new EnumeratorWithPrevious&lt;T&gt;(source);
        while (enumerator.MoveNext())
        {
            if (!predicate(enumerator.Previous, enumerator.Current))
            {
                yield return CreateNewGroup(enumerator.Current);
            }
            Add(enumerator.Current);
        }
        yield return CreateNewGroup(enumerator.Current);
    }
    catch (Exception e)
    {
        Log.Error(e); 
        throw;
    }
    finally
    {
        Log.Debug(""GroupWhile has ended"");
    }
}
</pre>
<p>Věřím, že ano. Takováto implementace má ale několik problémů: 
<ul>
<li>Metoda je příliš dlouhá. 
<li>Porušuje <a href=""http://goo.gl/GMV36B"">single responsibility principle</a>. 
<li>Většina kódu metody se stará a ošetření vstupních parametrů a logování a ne o to, co má metoda skutečně dělat. 
<li>Je velmi pravděpodobné, že další metody se budou stejným způsobem starat o ošetření vstupních parametrů a logování, a tím budeme porušovat pravidlo <a href=""http://goo.gl/BC3YWT"">don''t repeat yourself</a></li></ul>
<p>Díky aspektově orientovanému programování se dá metoda přepsat do této podoby. </p><pre class=""brush: csharp"">[Log] 
public IEnumerable&lt;IGrouping&lt;TKey, T&gt;&gt; GroupWhile(
    [Required] IEnumerable&lt;T&gt; source,
    [Required] Func&lt;T, T, bool&gt; predicate,
    [Required] Func&lt;T, T, TKey&gt; keyOfGroup)
{
    var enumerator = new EnumeratorWithPrevious&lt;T&gt;(source);
    while (enumerator.MoveNext())
    {
        if (!predicate(enumerator.Previous, enumerator.Current))
        {
            yield return CreateNewGroup(enumerator.Current);
        }
        Add(enumerator.Current);
    }
    yield return CreateNewGroup(enumerator.Current);
}
</pre>
<p>Zápis je snáze čitelný a o víc než polovinu kratší. Jak je to možné a jak to funguje, vám ukáži v sérii článků o aspektově orientovaném programování. Ukážeme základy aspektově orientovaného programovaní za použití frameworku Postsharp. Většinu příkladů budu přejímat ze stránek <a href=""http://goo.gl/jkjFXT"">Postsharpu</a>. 
<h3>Co je to aspektově orientované programování</h3>
<p>Dalo by se říci že aspektově orientované programování je nástavbou objektově orientovaného programování. Hlavním cílem aspektově orientované programování je eliminovat opakující se části kódu. Nejvíce aspekty využijeme pro úkoly jako je logování, exception handling, řízení přístupu podle uživatelských práv, cacheování, implementace INotifyPropertyChanged a mnoho dalších podobných úkolů. 
<h3>Základní terminologie</h3>
<ul>
<li><b>Joinpoint</b> 
<ul>
<li>Místo v kódu, které je možné obalit nějakou logikou pomocí Advice. 
<li>Například volání metod, inicializace třídy, přístup k vlastnosti instance třídy.</li></ul>
<li><b>Advice</b> 
<ul>
<li>Kód, kterým se obalí joinpoint. 
<li>Například zápis do logu</li></ul>
<li><b>Pointcut</b> 
<ul>
<li>Je množina joinpointů, které jsou obaleny stejnými advice 
<li>Například všechny metody z assembly budou používat stejné logování.</li></ul>
<li><b>Mixin</b> 
<ul>
<li>Výsledný objekt, po spojení advice a joinpoint</li></ul>
<li><b>Target</b> 
<ul>
<li>Objekt, který je ovlivněn aspektem</li></ul></li></ul>
<h3>Praktická ukázka</h3>
<p>Dost teorie pojďme si aspektově orientované programování ukázat v praxi. Předpokládejme, že máme metodu a potřebujeme zjistit, kdy došlo k jejímu volání.</p><pre class=""brush: csharp""><p>private static void SayHello()<br>{<br>&nbsp;&nbsp;&nbsp; Console.WriteLine(""Hello, world."");<br>}</p></pre>
<p>Nejjednodušším způsobem, jak toho docílit, je přidat výpis do trace. </p><pre class=""brush: csharp""><p>private static void SayHello()<br>{<br>&nbsp;&nbsp;&nbsp; Trace.WriteLine(""SayHello has started!"");<br>&nbsp;&nbsp;&nbsp; Console.WriteLine(""Hello, world."");<br>&nbsp;&nbsp;&nbsp; Trace.WriteLine(""SayHello has ended"");<br>}</p></pre>
<p>Tohle je sice funkční řešení, ale porušuje single responsibility principle a navíc nás nutí k opakování kódu. Předpokládejme, že máme ještě jednu metodu, u které taky chceme vědět, kdy se volá.</p><pre class=""brush: csharp"">private static void SayGoodBye()
{
    Trace.WriteLine(""SayGoodBye has started!"");
    Console.WriteLine(""Good bye, world."");
    Trace.WriteLine(""SayGoodBye has ended"");
}
</pre>
<p>Tohle by se dalo vyřešit jednoduchou dekorační funkcí. </p><pre class=""brush: csharp"">private static void TraceInfoDecorator(Action action)
{
    Trace.WriteLine(string.Format(""{0} has started!"", action.Method.Name));
    action();
    Trace.WriteLine(string.Format(""{0} has ended"", action.Method.Name));
}

private static void SayHello()
{
    Console.WriteLine(""Hello, world."");
}

private static void SayGoodBye()
{
    Console.WriteLine(""Good bye, world."");
}
</pre>
<p>Čímž přestaneme porušovat single responsibility principle a začneme dodržovat i don''t repeat yourself, jenže funkci nesmíme volat přímo, museli bychom používat volání: </p><pre class=""brush: csharp"">TraceInfoDecorator(SayHello);
TraceInfoDecorator(SayGoodBye);
</pre>
<p>Což není zrovna praktické. Teď už se konečně dostáváme k aspektově orientovanému programování. Ve výsledku naše metody budou vypadat takhle. </p><pre class=""brush: csharp"">[TraceInfo]
private static void SayHello()
{
    Console.WriteLine(""Hello, world."");
}

[TraceInfo]
private static void SayGoodBye ()
{
    Console.WriteLine(""Good bye, world."");
}
</pre>
<p>Nejdříve si vytvoříme TraceInfo atribut. Atribut je v C# třída, která dědí od System.Attribute a je dobrým zvykem přidat na konec názvu Attribute. </p><pre class=""brush: csharp"">public sealed class TraceInfoAttribute : Attribute { }
</pre>
<p>Aby atribut byl zároveň postsharpovým aspektem, musí implementovat rozhraní IAspect ze jmenného prostoru PostSharp.Aspects a musí být serializovatelný. Ve stejném jmenném prostoru se nachází mnoho tříd pro usnadnění vytváření aspektů. Dnes použijeme třídu OnMethodBoundaryAspect. Je potřeba mít ve Visual Studiu doinstalovaný doplněk PostSharp. Dá se stáhnout přímo z Visual Studia. Automaticky se naistaluje ve verzi free. Po instalaci je potřeba restartovat Visual Studio. </p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn---vod_FB5E/image_8.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn---vod_FB5E/image_thumb_3.png"" width=""610"" height=""422""></a></p>
<p>Dále je potřeba přidat PostSharp do projektu. Stačí kliknout pravým na projekt v solution exploreru a zvolit ""Add PostSharp to project"". Upravíme náš atribut tak aby dědil od třídy OnMethodBoundaryAspect, přidáme mu atribut Serializable.</p><pre class=""brush: csharp"">[Serializable] 
public sealed class TraceInfoAttribute : OnMethodBoundaryAspect { }
</pre>
<p>OnMethodBoundaryAspect se postará o obalení metody blokem try a přidá šest virtuálních metod. Jejich volání vypadá takhle. </p><pre class=""brush: csharp"">OnEntry(args);
try
{
    //Kód metody 
    //…
    OnSuccess(args);
}
catch (Exception)
{
    OnException(args);
    throw;
}
finally
{
    OnExit(args);
} 
</pre>
<p>Parametr args předávaný metodám je typu PostSharp.Aspects.MethodExecutionArgs. Dále jsou zde další dvě metody OnYield a OnResume jak název napovídá, OnYield se volá při vyskakování z metody pomocí yield return nebo await a metody OnResume se volá při vracení do metody. </p>
<p>Naimplementujeme metody OnEntry a OnExit. </p><pre class=""brush: csharp"">[Serializable] 
public sealed class TraceInfoAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""{0} has started!"", GetMethodFullName(args)));
    }
 
    public override void OnExit(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""{0} has ended"", GetMethodFullName(args)));
    }
 
    private static string GetMethodFullName(MethodExecutionArgs args)
    {
        return string.Format(""{0}.{1}.{2}"",
            args.Method.DeclaringType.Namespace,
            args.Method.DeclaringType.Name,
            args.Method.Name);
    }
}
</pre>
<p>Náš aspekt je již funkční, že funguje si můžeme jednoduše ověřit.</p><pre class=""brush: csharp"">static void Main(string[] args)
{
    Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

    SayHello();
    SayGoodBye();
}
</pre>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn---vod_FB5E/image_10.png""><img title=""vystup"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn---vod_FB5E/image_thumb_4.png"" width=""464"" height=""284""></a></p>
<p>No a to je vše. Příště se podíváme, jak jednoduše se s PostSharpem dá udělat logování. </p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (6395, 16, CAST(N'2015-12-20T00:00:00.000' AS DateTime), N'Aspektově orientované programování – Logování', N'<p>Jak do projektu jednoduše zavést logování vám ukážu na jednoduché ukázkové aplikaci. Zdrojové soubory si můžete stáhnout <a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/PostSharp2OnlyApp_1.zip"" target=""_self"">zde</a>.</p> <p>PostSharp nám nabízí velmi jednoduchý způsob jak zavézt logování, téměř bez práce. Klikneme na název metody a rozbalíme možnosti, které nám Visual Studio nabízí. Standartní klávesová zkratka je <strong>ctrl</strong>+<strong>.</strong>. Ze seznamu vybereme Add logging. </p> <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_12.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_thumb_1.png"" width=""610"" height=""327""></a></p> <p>Zobrazí se nám dialog pro nastavení logování. Když klikneme na edit u profilu máme možnost nastavit, co a s jakou prioritou se bude zapisovat do logu. </p> <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_6.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_thumb_2.png"" width=""610"" height=""480""></a></p> <p>V dalšímu kroku, máme na výběr back end, který chceme k logování použít. Já jsem zvolil knihovnu Log4Net.</p> <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_8.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_thumb_3.png"" width=""610"" height=""480""></a></p> <p>V další obrazovce je shrnutí co vše PostSharp přidá do projektu.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_10.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_thumb_4.png"" width=""610"" height=""480""></a></p> <p>Následuje instalace a dokončení.  <p>K metodě AddEmailAddress se přidal atribut Log a do projektu se přidal soubor PostSharp2.pssln, který obsahuje nastavení logování.</p><pre class=""brush: csharp"">[Log]
public void AddEmailAddress(string email)
{
    if (email.IsEmail())
    {
        emailList.Add(email);
    }
    else
    {
        throw new EmailAddressInInvalidFormat(email);
    }
}
</pre>
<p>Ještě potřebujeme nastavit Log4Net. Do App.config přidáme dvě sekce. Sekce configSection musí být uvedena jako první sekce hned za tagem <b>&lt;configuration&gt;</b>. Druhou sekcí je nastavení Log4Net. Výsledný App.config vypadá takhle:</p><pre class=""brush: xml"">&lt;?xml version=""1.0"" encoding=""utf-8"" ?&gt;
&lt;configuration&gt;
  &lt;configSections&gt;
    &lt;section name=""log4net"" type=""log4net.Config.Log4NetConfigurationSectionHandler,log4net, Culture=neutral"" /&gt;
  &lt;/configSections&gt;

  &lt;startup&gt;
    &lt;supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5"" /&gt;
  &lt;/startup&gt;
 
  &lt;log4net&gt;
    &lt;appender name=""FileAppender"" type=""log4net.Appender.FileAppender,log4net""&gt;
      &lt;file value=""Log.txt"" /&gt;
      &lt;appendToFile value=""true"" /&gt;
      &lt;lockingModel type=""log4net.Appender.FileAppender+MinimalLock"" /&gt;
      &lt;layout type=""log4net.Layout.PatternLayout""&gt;
        &lt;conversionPattern value=""%date [%thread] %level %logger - %message%newline"" /&gt;
      &lt;/layout&gt;
      &lt;filter type=""log4net.Filter.LevelRangeFilter""&gt;
        &lt;levelMin value=""INFO"" /&gt;
        &lt;levelMax value=""FATAL"" /&gt;
      &lt;/filter&gt;
    &lt;/appender&gt;
    &lt;root&gt;
      &lt;level value=""DEBUG"" /&gt;
      &lt;appender-ref ref=""FileAppender"" /&gt;
    &lt;/root&gt;
  &lt;/log4net&gt;
&lt;/configuration&gt;
</pre>
<p>Ještě musíme do AssemblyInfo.cs přidat dva řádky aby se Log4Net nastavil. </p><pre class=""brush: csharp"">using log4net.Config;

[assembly: XmlConfigurator(Watch = true)]
</pre>
<p>Aplikaci můžeme spustit</p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_18.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--Logovn_6BCC/image_thumb_7.png"" width=""610"" height=""325""></a></p>
<p>vytvoří se nám soubor Log.txt a bude obsahovat jeden řádek.</p><pre class=""brush: text"">2014-12-07 22:15:14,549 [1] ERROR PostSharp2.Model.User -&nbsp;&nbsp; EmailAddressInInvalidFormatException: User.AddEmailAddress(this = {John Doe john@doe.it}, string email = ""john.doe.gmail.com"")
</pre>
<h2>Hromadné použití aspektů</h2>
<p>Logování už je funkční, jen by se nám asi moc nelíbilo ke každé funkci dávat atribut [Log]. Proto existuje několik způsobů jak použít atribut plošně. Můžeme uvést atribut nad třídou a tím se aplikuje na všechny její metody. </p><pre class=""brush: csharp"">[Log]
public class JsonUserService : IUserService
</pre>
<p>To pro naše použití, ale není stále dostatečné. Máme možnost nasadit aspekt na celý jmenný prostor. Do AssemblyInfo.cs přidáme následující dva řádky a smažeme atribut log z metody AddEmailAddress, protože již nebude potřeba. </p><pre class=""brush: csharp"">using PostSharp.Patterns.Diagnostics;

[assembly: Log(AttributeTargetTypes = ""PostSharp2.*"", AttributePriority = 1)]
</pre>
<p>Tím použijeme aspekt Log na všech metodách ve jmenném prostoru PostSharp2 a ve všech vnořených jmenných prostorech. </p>
<h2>Možnosti filtrování</h2>
<p>Ne vždy chceme nastavit atribut úplně na všem. A proto jsou zde široké možnosti filtrování. Parametr AttributeExclude říká, že pro metody vyhovující filtru se aspekt Log nenastaví. AttributePriority určuje pořadí, ve kterém jsou aspekty aplikovány. Dále můžeme použít prefix regex díky tomu můžeme použít pro filtrování regulární výraz. </p><pre class=""brush: csharp"">[assembly: Log(
    AttributePriority = 2,
    AttributeExclude = true,
    AttributeTargetTypes = @""regex:PostSharp2.Extensions.*"")]
</pre>
<p>Také je možné použít filtrování podle modifikátorů tříd. Takhle by vypadalo pravidlo přidávající aspekt všem statickým třídám z jmenného prostoru PostSharp2. </p><pre class=""brush: csharp"">[assembly: Log(AttributeTargetTypes = ""PostSharp2.*"",
               AttributeTargetTypeAttributes = MulticastAttributes.Static)]
</pre>
<p>Obdobným způsobem je možné filtrovat podle signatury metody. Například všechny metody s ref parametrem z jmenného prostoru PostSharp2. </p><pre class=""brush: csharp"">[assembly: Log(AttributeTargetTypes = ""PostSharp2.*"",
               AttributeTargetMemberAttributes = MulticastAttributes.RefParameter)]
</pre>
<p>V případě že by vám nestačily tyhle možnosti filtrování je možnost u Aspektu přepsat metodu CompileTimeValidate. </p><pre class=""brush: csharp"">public override bool CompileTimeValidate(MethodBase method)
{
    Type targetType = method.DeclaringType;
    if (typeof(IDemandAdminRightToAccess).IsAssignableFrom(targetType))
    {
        Message.Write(targetType, SeverityType.None, ""NotSupportedType"", ""The target type does not implement IUserService."");
        return true;
    }
    return false;
}
</pre>
<p>Takhle se dá aplikovat aspekt na všechny třídy, které implementují dané rozhraní. Velmi užitečné je to například v případě práv, kdy stačí přidat třídě interface a o zbytek se postaráme v aspektu. </p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (6408, 16, CAST(N'2016-01-15T00:00:00.000' AS DateTime), N'Aspektově orientované programování – INotifyPropertyChanged', N'<p>Dnes si ukážeme jak snadně naimplementovat INotifyPropertyChanged. </p> <p>Všichni co dělali z wpf to určitě znají dlouhý a špatně přehledný kód modelů, který vypadá nějak takto: </p><pre class=""brush: csharp"">class User : INotifyPropertyChanged
{
    private string firstName;
    private string lastName;
    private string phone;

    public event PropertyChangedEventHandler PropertyChanged;

    public string FirstName
    {
        get { return firstName; }
        set
        {
            if (value != firstName)
            {
                firstName = value;
                OnPropertyChanged();
                OnPropertyChanged(""FullName"");
            }
        }
    }

    public string LastName
    {
        get { return lastName; }
        set
        {
            if (value != lastName)
            {
                lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(""FullName"");
            }
        }
    }

    public string FullName
    {
        get { return string.Format(""{0} {1}"", this.FirstName, this.LastName); }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
</pre>
<p>Rozhraní INotifyPropertyChanged se dá velmi pohodlně naimplementovat pomocí PostSharpu. </p>
<h2>Implementace INotifyPropertyChanged</h2>
<p>Vytvoříme třídu s požadovanými vlastnostmi: </p><pre class=""brush: csharp"">internal class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName
    {
        get { return string.Format(""{0} {1}"", this.FirstName, this.LastName); }
    }
}</pre>
<p>Obdobně jako v případě přidání logování klikneme na název třídy a rozbalíme nabídku možností (<b>CTRL</b>+<b>.</b>). Zvolíme možnost “Implement INotifyPropertyChanged”</p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image_2.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image_thumb.png"" width=""610"" height=""328""></a>&nbsp;</p>
<p>Při první implementaci INotifyPropertyChanged v projektu potřebuje PostSharp stáhnout balíček PostSharp.Patterns.Model. O tom nás informuje jednoduchým dialogem.</p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image3.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image3_thumb.png"" width=""610"" height=""480""></a></p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image6.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image6_thumb.png"" width=""610"" height=""480""></a></p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image9.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image9_thumb.png"" width=""610"" height=""480""></a></p>
<p>Ke třídě se připojí atribut NotifyPropertyChanged:</p><pre class=""brush: csharp"">[NotifyPropertyChanged]
internal class User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName
    {
        get { return string.Format(""{0} {1}"", this.FirstName, this.LastName); }
    }
}</pre>
<p>No a tím máme hotovo. Ovšem má to malou záludnost. Wpf binding funguje správně, ale nemůžeme se navázat na event PropertyChanged ze C# kódu, jednoduše proto, že ho tam v době kompilace nemáme. PostSharp ho tam doplní až po kompilaci. Pokud potřebujeme někde v kódu reagovat na událost PropertyChanged, musíme použít přetypování na INotifyPropertyChanged.</p><pre class=""brush: csharp"">User user = new User { FirstName = ""John"", LastName = ""Doe"" };
((INotifyPropertyChanged)user).PropertyChanged += user_PropertyChanged;
</pre>
<p>To se ovšem nezdá Visual Studiu a reaguje hláškou:</p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image1.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image1_thumb.png"" width=""610"" height=""52""></a></p>
<p>PostSharp má pro podobné situace metodu Post.Cast&lt;SourceType, TargetType&gt;. Hlavní výhodou použití téhle metody o proti klasickému přetypování je, že v případě kdy nebude přetypování možné, vyhodí PostSharp chybu při kompilaci, kdežto klasické přetypování jí vyhodí až za běhu programu.</p><pre class=""brush: csharp"">User user = new User { FirstName = ""John"", LastName = ""Doe"" };
Post.Cast&lt;User, INotifyPropertyChanged&gt;(user).PropertyChanged += user_PropertyChanged;
</pre>
<h2>Post kompilace</h2>
<p>Už jsme si ukázali jak pomocí PostSharpu logovat, jak naimplementovat INotifyPropertyChanged, ale neukázali jsme si jak funguje. Jak už jsem zmínil výše, aspekty se připojují až po kompilaci. Projekt se nejdříve zkompiluje, postsharp post processor pak vezme vygenerovanou assembly, pomocí MSIL injection připojí aspekty a vygeneruje novou výslednou assembly.</p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image12.png""><img title="""" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Aspektov-orientovan-programovn--INotifyP_141A3/image12_thumb.png"" width=""610"" height=""146""></a> 
<p>Pojďme se podívat, co generuje PostSharp. Použiju jednoduchý aspekt podobný jako byl v prvním dílu.</p><pre class=""brush: csharp""> 
[Serializable]
public sealed class TraceInfoAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""Entering {0} with parametr {1}"", GetMethodFullName(args), ArgumentsToString(args)));
    }

    public override void OnException(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""Exception {0} on method {1}"", args.Exception, GetMethodFullName(args)));
    }

    public override void OnSuccess(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""Success {0}"", GetMethodFullName(args)));
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Trace.WriteLine(string.Format(""Leaving {0}"", GetMethodFullName(args)));
    }

    private static string ArgumentsToString(MethodExecutionArgs args)
    {
        ParameterInfo[] methodParams = args.Method.GetParameters();
        List<string> paramsList = new List<string>();
        for (int x = 0; x &lt; methodParams.Count(); x++)
        {
            paramsList.Add(string.Format(""{0}: {1} = {2}"",
                methodParams[x].ParameterType.FullName,
                methodParams[x].Name,
                args.Arguments[x]));
        }
        return string.Join("", "", paramsList);
    }

    private static string GetMethodFullName(MethodExecutionArgs args)
    {
        return string.Format(""{0}.{1}.{2}"",
            args.Method.DeclaringType.Namespace,
            args.Method.DeclaringType.Name,
            args.Method.Name);
    }
}
</pre><pre class=""brush: csharp"">[TraceInfo]
private static void SayHello(string who, int howManyTimes)
{
    for (int i = 0; i &lt; howManyTimes; i++)
    {    
        Console.WriteLine(""Hello, {0}."", who);
    }
}
</pre>
<p>Když metodu SayHello, k níž je aspekt připojen, dekompilujem Reflectorem nebo jiným dekompilátorem, uvidíme:</p><pre class=""brush: csharp"">private static void SayHello(string who, int howManyTimes)<br>{<br>&nbsp;&nbsp;&nbsp; Arguments&lt;string, int&gt; arguments = new Arguments&lt;string, int&gt; {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Arg0 = who,<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Arg1 = howManyTimes<br>&nbsp;&nbsp;&nbsp; };<br>&nbsp;&nbsp;&nbsp; MethodExecutionArgs args = new MethodExecutionArgs(null, arguments) {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Method = &lt;&gt;z__a_1._2<br>&nbsp;&nbsp;&nbsp; };<br>&nbsp;&nbsp;&nbsp; &lt;&gt;z__a_1.a0.OnEntry(args);<br>&nbsp;&nbsp;&nbsp; try<br>&nbsp;&nbsp;&nbsp; {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; for (int i = 0; i &lt; howManyTimes; i++)<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(""Hello, {0}."", who);<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;&gt;z__a_1.a0.OnSuccess(args);<br>&nbsp;&nbsp;&nbsp; }<br>&nbsp;&nbsp;&nbsp; catch (Exception exception)<br>&nbsp;&nbsp;&nbsp; {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; args.Exception = exception;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;&gt;z__a_1.a0.OnException(args);<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; throw;<br>&nbsp;&nbsp;&nbsp; }<br>&nbsp;&nbsp;&nbsp; finally<br>&nbsp;&nbsp;&nbsp; {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;&gt;z__a_1.a0.OnExit(args);<br>&nbsp;&nbsp;&nbsp; }<br>}</pre>
<p>Pojďme si kód trošku rozebrat. Proměnná methodExecutionArgs obsahuje informace o metodě. Konstruktoru se předává null, protože první parametr je instance objektu na kterém je metoda vyvolána. Druhým jsou parametry metody, pokud je metoda bezparametrická a nebo aspekt paremetry nepotřebuje, bude i druhý paramter null. Do vlastnosti Method proměnné methodExecutionArgs se vloží informace o metodě jako je jmený prostor, název atd... Vlastnost je typu <a href=""http://doc.postsharp.net/t_postsharp_aspects_methodexecutionargs"">MethodExecutionArgs</a>.</p>
<p>&lt;&gt;z__a_1 je třída, ve které si PostSharp udržuje informace o třídách a metodách a instance atributů. Pro každou třídu a každou metodu je zde statická properity s informacemi. Například &lt;&gt;z__a_1._2 jsou informace o metodě SayHello a &lt;&gt;z__a_1.a0 je instance TraceInfoAttribute použitá v metodě SayHello.</p><pre class=""brush: csharp"">[CompilerGenerated, DebuggerNonUserCode]
internal sealed class &lt;&gt;z__a_1
{
    // Fields
    internal static MethodBase _2;
    internal static readonly TraceInfoAttribute a0;

    // Methods
    [CompilerGenerated]
    static &lt;&gt;z__a_1();
}
</pre>
<h2>Optimalizace aspektů</h2>
<p>PostSharp je naštěstí dost chytrý na to, aby dělal jen to co je opravdu potřeba. Pokud zjednodušíme aspekt.</p><pre class=""brush: csharp"">[Serializable]
public sealed class TraceInfoAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Trace.WriteLine(""Entry"");
    }
}
</pre>
<p>Bude kód metody SayHello takový to:</p>
<pre class=""brush: csharp"">
private static void SayHello(string who, int howManyTimes)
{
    &lt;&gt;z__a_1.a0.OnEntry(null);
    for (int i = 0; i &lt; howManyTimes; i++)
    {
        Console.WriteLine(""Hello, {0}."", who);
    }
}</pre>
<p>Všiměte si že zmizela počáteční inicializace Arguments i MethodExecutionArgs a blok try catch, protože nejsou pro aktuální aspekt potřeba. Bohužel PostSharp provádí optimalizaci až od verze Professional. Ve verzi express se musíme spokojit s voláním prázdných metod.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (6415, 16, CAST(N'2016-05-07T00:00:00.000' AS DateTime), N'LINQ – Rozšíření jazyka C#', N'<p>Minule jsme si představili technologii LINQ, v tomhle díle si ukážeme, jak se změnil jazyk C#, aby nám usnadnili dotazování s LINQem. Žádná z nově přidaných možností jazyka není pro použití LINQ nezbytně nutná, jedná se hlavně o syntaktický cukr pro usnadnění dotazování.</p> <h2>Anonymní metody</h2> <p>Anonymní metody byly přidány už do C# 2.0. Anonymní metody nám dovolují definovat funkce přímo tam, kde je potřebujeme. Pro snazší pochopení pomůže ukázka.</p> <p>Kód bez anonymních metod:</p><pre class=""brush:csharp"">static void Main(string[] args)
{
	var query = Orders.Where(Predicate);
}
private static bool Predicate(Order o)
{
	return o.Price &gt; 1000;
}
</pre>
<p>Pro usnadnění přibyly anonymní metody.</p><pre class=""brush:csharp"">var query = Orders.Where(delegate(Order o) { return o.Price &gt; 1000; });
</pre>
<p>Kompilátor si signaturu funkce vygeneruje sám. </p>
<h2>Lambda výrazy</h2>
<p>Lambda výrazy zkracují zápis Anonymní metody. Zápis pomocí lambda výrazu vypadá takto:</p><pre class=""brush:csharp"">var query = Orders.Where(o =&gt; o.Price &gt; 1000); //Lambda výraz s jedním parametrem
var query = Orders.Where((o, i) =&gt; o.Price &gt; i); //Lambda  výraz se dvěma parametry
</pre>
<h2>Extension methods</h2>
<p>Extension methods nám dovolují přidat metodu k existujcímu datovému typu a tím ho rozšířit. Na pozadí fungují tak, že instanci typu, který mají rozšířit, přejímají jako první parametr. Extension methods nemůžou přistupovat k private a protected prvkům daného typu. Musí být ve statických třídách a musí být statické, datový typ, který rozšiřují, je uveden jako první parametr s klíčovým slovem <b>this</b>. </p><pre class=""brush:csharp"">static void Main(string[] args)
{
	Random rand = new Random();
	int phoneNumber = rand.NextTelephoneNumber();
}
<br>
public static class RandomExtensions
{
	public static int NextTelephoneNumber(this Random random)
	{
		List&lt;int&gt; prefixs = new List&lt;int&gt; { 736, 775, 604, 605, 608, 730 };
		int prefix = prefixs[random.Next(0, prefixs.Count)];
		return prefix * 1000000 + random.Next(100000, 1000000);
	}
}
</pre>
<p>Právě na Extension methods si LINQ hodně zakládá, hodně to zpřehledňuje kód. Právě díky nim máme pocit, že metody LINQu jsou přímo metodami instance.</p><pre class=""brush:csharp"">IList&lt;Order&gt; orders = new List&lt;Order&gt;();
var countOfOrdersWithPriceOverThousand = orders.Count(o =&gt; o.Price &gt; 1000);
</pre>
<h2>Anonymní datové typy</h2>
<p>Možná jste si toho všimli už v prvním díle. Enumerátor jakého datového typu je v query? </p><pre class=""brush:csharp"">var query = from o in Orders join c in Customers on o.CustomerID equals c.ID
			where o.Price &gt; 1000
			select new {c.Name, c.Address, o.Price};
</pre>
<p>Jedná se anonymní datový typ, nejedná se o dynamický typ, jako je <strong>dynamic</strong>, pouze nikde neuvádíme jeho definici, opět ji za nás dodělá kompilátor. Zvláštní je jaké dává kompilátor jména, při pohledu Reflektorem můžeme vidět jména typů jako ''&lt;&gt;f__AnonymousType1`3''&lt;int, int, decimal&gt;. Kompilátor generuje anonymní typy jako generické a pak jejich typy parametrizuje, aby předcházel vytváření zbytečného množství datových typů.</p>
<p>Properities anonymního datového typu můžeme i pojmenovávat:</p><pre class=""brush:csharp"">var query = from o in Orders join c in Customers on o.CustomerID equals c.ID
			where o.Price &gt; 1000
			select new {CustomerID = c.ID, OrderID = o.ID, c.Name, c.Address, o.Price};

foreach (var item in query)
{
    Console.WriteLine(""{0} {1} {2} {3} {4}"",
                        item.CustomerID,
                        item.OrderID,
                        item.Name,
                        item.Address,
                        item.Price);
}
</pre>
<h2>Implicitně typovaná lokální proměnná</h2>
<p>Anonymní datové typy by byly k ničemu, kdybychom je neměli jak použít. Proto se C# 3.0 přišlo také klíčové slovo var. Pokud použijeme var, tak za nás kompilátor sám doplní datový typ, pokud ho dokáže jednoznačně určit z výrazu. Datový typ se určuje pouze jednou, pak ho nelze změnit. Následující kód je neplatný! </p><pre class=""brush:csharp"">var count = 5; //datový typ se implicitně určí jako int
count = ""pět stránek""; //tohle nelze, count je typu int
</pre>
<p>Správné použití var: </p><pre class=""brush:csharp"">foreach (var item in query)
{
	Console.WriteLine(""Name: {0}\tAdrress: {1}\t\tPrice: {2}"", item.Name, item.Address, item.Price);
}
</pre>
<p>V tomhle případě datový typ položky item neznáme, proto necháme kompilátor, aby ho za nás doplnil. Intellisense pro anonymní datové typy, i implicitně definované typy, funguje. Silná typovost je zachována.</p>
<h2>Dotazovací výrazy</h2>
<p>Dotazovací výrazy nám umožňují zapisovat LINQ dotazy, tak aby se co podobali SQL syntaxi. Existuje sada klíčových slov <strong>from</strong>, <strong>in</strong>, <strong>where</strong>, <strong>let</strong>, <strong>join</strong>, <strong>on</strong>, <strong>equals</strong>, <strong>into</strong>, <strong>orderBy</strong>, <strong>descending</strong>, <strong>ascending</strong>, <strong>select</strong>, <strong>group by</strong>. Jedná se o pouhý syntaktický cukr, kompilátor přeloží klauzule na posloupnost volání LINQ metod. </p><pre class=""brush:csharp"">var query = from c in customerList
            where c.City == ""London""
            select c; 
//se přeloží jako            
var query = customerList.Where(c =&gt; c.City == ""London"");</pre><pre class=""brush:csharp"">var custSuppliers =
    from cust in customerList
    join sup in supplierList on cust.Country equals sup.Country into ss
    from s in ss.DefaultIfEmpty()
    select new
    {
        CompanyName = cust.CompanyName,
        SupplierName = s == null ? string.Empty : s.SupplierName,
        Country = s == null ? string.Empty : s.City,
    };
//se přeloží jako
var custSuppliers = customerList.GroupJoin(supplierList, cust =&gt; cust.Country, sup =&gt; sup.Country, (cust, ss) =&gt; new {cust, ss})
                    .SelectMany(t =&gt; t.ss.DefaultIfEmpty(), (t, s) =&gt; new
                    {
                        CompanyName = t.cust.CompanyName,
                        SupplierName = s == null ? string.Empty : s.SupplierName,
                        Country = s == null ? string.Empty : s.City,
                    });</pre>
<p>Dalším významným rozšířením je <strong>yield return</strong>, ten bude popsán v příštím díle.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (7435, 19, CAST(N'2015-01-31T22:53:34.880' AS DateTime), N'MonoGame první nahlédnutí', N'<p>Ahoj poprvé. V následující sérii článků se pokusím poskytnout čtenářstvu něco málo o frameworku MonoGame a také o práci s 3D grafikou. A cože to to MonoGame je? To se pokusím vysvětlit právě v tomto článku.  <p>MonoGame je framework určený pro tvorbu her. Když už se rozhodnete, že vytvoříte hru, tak máte v zásadě několik možností co provést:  <ul> <li>napsat si kompletně vše sami (to potom určitě tyto články potřebovat nebudete),  <li>použít čisté DirectX a nebo OpenGL,  <li>použít herní framework (Slick pro javu kupříkladu, MonoGame pro C#),  <li>použít Unity,použít komerční velký a také často drahý engine (Cry Engine, UDK...).</li></ul> <p>Volby jsem naschvál seřadil podle náročnosti, která je zde kladená na programátora jako takového. MonoGame se tak nachází někde ve středu. Nemusíte ovládat grafické API, protože o to se MonoGame stará samo, ale pouze je krmíte daty. Na druhou stranu to není tak snadné jako v Unity, které se postará o většinu grafických efektů o management scény a tak dále. Je to prostě vprostřed.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/HorizontalLogo_64px_2.png""><img title=""MonoGame logo"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""MonoGame logo"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/HorizontalLogo_64px_thumb.png"" width=""240"" height=""51""></a>  <p>MonoGame nám tedy neposkytuje kompletní servis, ale zapouzdřuje nízko úrovňová volání, takže je snadno pochopitelný pro začátečníky. Navíc kdo znal XNA, tak zde bude jako ryba ve vodě. MonoGame je vlastně jen open source implementací tohoto API. A jde vlastně ještě dál, než kam se XNA odvážilo. Přidává totiž i podporu dalších platforem jmenovitě kupříkladu Android, iOS a nebo taky Linux. Hru lze tak s drobnými obměnami snadno přenášet mezi různými platformami. Což je v dnešním světe dost potřebná vlastnost. Dobré je ale taky upozornit, že projekt je stále ve vývoji a že u některých převážně pokročilejších funkcí můžeme narazit na oblíbenou NotImplementedException. Ale tato místa pomalu ale jistě mizí.</p> <div id=""scid:5737277B-5D6D-4f48-ABFC-DD9C333F4C5D:759d940b-b327-4b0a-ad29-f09ccb5e5e4c"" class=""wlWriterEditableSmartContent"" style=""width: 448px; float: none; padding-bottom: 0px; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; margin-right: auto""><div><object width=""448"" height=""252""><param name=""movie"" value=""http://www.youtube.com/v/mx4B8XFtnZo?hl=en&amp;hd=1""></param><embed src=""http://www.youtube.com/v/mx4B8XFtnZo?hl=en&amp;hd=1"" type=""application/x-shockwave-flash"" width=""448"" height=""252""></embed></object></div><div style=""width:448px;clear:both;font-size:.8em"">Hry, které využívají MonoGame</div></div> <p>K práci v MonoGame budeme potřebovat Visual Studio 2013 (a nebo jinou vaši oblíbenou verzi i taková C# express edice by měla stačit, v pohodě mi běželo MonoGame i ve verzi 2010) a to je vlastně všecko.  <h2>Instalace</h2> <p>Před použitím budeme potřebovat buď MonoGame nainstalovat z připraveného instalátoru, který je k nalezení na webu projektu (<a href=""http://www.monogame.net/downloads/"">http://www.monogame.net/downloads/</a>) a nebo můžeme použít hotového NuGet balíčku (toho využijeme v tomto návodu). Ale konec řečí pojďme na to.  <h2>První “hra”</h2> <p>Ve Visual Studiu si založíme prázdný projekt. V záložce References pak vybereme Manage NuGet Packages a dostaneme se do dialogového okna, kde si vyhledáme balíček MonoGame a nainstalujeme si jej.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/nu-get_2.png""><img title=""NuGet manager"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""NuGet manager"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/nu-get_thumb.png"" width=""524"" height=""349""></a>  <p>Přímo po instalaci se nám připraví projekt s jednoduchou hrou. Pokud jej nyní zkusíte spustit, tak by se mělo objevit okno s modrou obrazovkou a taky bohužel okno s konzolí. Modré okno chceme to je místo, kde budeme dělat naše grafické zázraky, ale konzoli už tolik ne. Naštěstí se jí dá snadno zbavit, v nastavení projektu si pod záložkou<em> Application</em> změníme u položky<em> Output type</em> z<em> Console Application</em> na <em>Windows Application</em> a konzole nám zmizí.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/settings_2.png""><img title=""Změna nastaven&iacute; projektu"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""Změna nastaven&iacute; projektu"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/settings_thumb.png"" width=""590"" height=""312""></a>  <p>Pojďme se ale podívat přímo do zdrojového kódu, ten nám o samotné aplikaci prozradí více. V projektu máme celkem dva různé zdrojové soubory <em>Program.cs</em> a <em>Game1.cs</em>. To zajímavé se nachází v <em>Game1.cs</em>, tak se na něj pojďme podívat detailně. Právě v této třídě budeme mít celou logiku naší hry. Popravdě to ze začátku bude hře velmi vzdálené, ale třeba se to časem vyvrbí. Máme zde několik přepsaných metod, pojďme se na ně podívat.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/smycka_2.png""><img title=""Hern&iacute; smyčka"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""Hern&iacute; smyčka"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/smycka_thumb.png"" width=""423"" height=""317""></a>  <p>První je metoda <em>Initialize</em>, tato metoda se nám zavolá jednou a to pouze při startu celé aplikace. Sem budeme umisťovat jak už jméno jasně říká inicializační věci. Metoda <em>LoadContent</em> se volá taktéž jen jednou a také po startu aplikace, ale až po metodě <em>Initialize</em>. Rozdíl mezi těmito metodami je ten, že v tento okamžik už máme vše připraveno pro načítání součástí hry (textury, zvuky, modely...) a veškeré načítání budeme umisťovat sem. Metoda Update je narozdíl od předchozích metod volána až do konce hry. V této metodě budeme měnit stav objektů ve hře, třeba řídit animaci. Předposlední metodou je metoda <em>Draw</em>, právě zde budeme realizovat vykreslování. Obě tyto metody se volají v nekonečném cyklu po celou dobu běhu programu. Poslední metoda <em>UnloadContent</em> pak slouží k vyhození načteného obsahu a zavolá se jednou, při vypínání hry.  <p>Ale aby nebyl tento článek jen teoretickým úvodem pojďme si na modré pozadí (těm co znají staré XNA, tak tuto barvu znají moc dobře, je to ona slavná corn flower blue) také něco vykreslit. Nebudeme začínat hned nějakým složitým modelem, ale ukážeme si vykreslení takzvaného sprite. Sprite je vlastně jen obrázek. V MonoGame na to máme stejně jako v XNA k dispozici třídu <em>SpriteBatch</em>. Tato třída nám vše zařídí a ani nemusíme mít žádné tušení jak to uvnitř funguje. My ale chceme jít trošku více do hloubky a tak si to v dalších dílech osvětlíme. V šabloně projektu již máme jednu instanci připravenou, takže ji jen v metodě <em>Draw</em> požijeme. Nejdřív, ale musíme mít co vykreslovat. A k tomu potřebujeme do projektu přidat obrázek. Proto si v projektu vytvoříme složku Content a do ní jej nahrajeme. V nastavení nesmíme zapomenout změnit hodnotu u parametru <em>Copy to Output Directory</em> na <em>Copy if newer</em>, aby se nám při případné změně sám nakopíroval.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/logo-nas_2.png""><img title=""Přid&aacute;n&iacute; obr&aacute;zku"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""Přid&aacute;n&iacute; obr&aacute;zku"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/logo-nas_thumb.png"" width=""224"" height=""276""></a>  <p>Toto je jen jeden ze způsobů jak lze v MonoGame nahrávat obrázek o dalších možnostech si povíme něco v nějakém dalším dílu. Pojďme se podívat do kódu. Přidáme si proměnnou, kde budeme mít uloženou texturu:</p><pre class=""brush:csharp"">Texture2D texture;</pre>
<p>A v metodě <em>LoadContent</em> ji jak jsme si řekli výše nahrajeme:</p><pre class=""brush:csharp"">texture = Content.Load&lt;Texture2D&gt;(""logo"");</pre>
<p>Všimněte si prosím, že název souboru je zde BEZ přípony a také bez složky Content. Toto si prosím zapamatujme, stejná pravidla pak platí i pro nahrávání modelů a dalších součástí hry. Metoda je generická a proto se pro jednotlivé typy herního obsahu bude tento parametr lišit. Máme tedy nahranou texturu a teď ji už jen chceme vykreslit. Vykreslení tedy dáme do metody <em>Draw</em>. Vypadá to následovně:</p><pre class=""brush:csharp"">Begin();
Draw(texture,new Vector2(100,100),Color.White);
End();</pre>
<p>Zavoláním metody <em>Begin</em> si incializujeme sprite batch a je tak připravena pro vykreslování. Na následujícím řádku, pak dáváme jakýsi záznam toho, co se má vykreslit. Konkrétně vykreslujeme naši texturu na pozici (100,100). Nulu pak máme v horním levém rohu. Poslední řádek pak provede vykreslení ač tomu název metody vůbec nenapovídá, je to tak. Sprite batch se totiž snaží o optimalizaci vykreslování a proto se o vykreslení postará až když má vše co vykreslovat má. Ale o tom, co vykreslování ovlivňuje a zpomaluje si povíme zase někdy příště. Nyní, když projekt spustíme měli bysme mít modrou plochu a na ní náš obrázek.</p>


<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/mg-prvni_2.png""><img title=""Prvn&iacute; aplikace"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: none; padding-top: 0px; padding-left: 0px; margin-left: auto; display: block; padding-right: 0px; border-top-width: 0px; margin-right: auto"" border=""0"" alt=""Prvn&iacute; aplikace"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/mg-prvni_thumb.png"" width=""563"" height=""359""></a> 
<p>A to bude pro dnešek všechno. Celý projekt si můžete stáhnout tady:&nbsp; <div id=""scid:6E335E69-603A-4F1E-AA4B-726D584133B4:5537034e-17a4-4dda-a6d1-7a943bbc18b9"" class=""wlWriterSmartContent"" style=""float: none; padding-bottom: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline; padding-right: 0px""><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/afb29f335a83_10886/eaecfa66-342a-4609-87e5-ae57697f7fb4_MG01.zip"">MG01.zip</a></div> Příště se podíváme trochu pod kapotu a zjistíme jak to všechno funguje.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (7436, 18, CAST(N'2015-02-13T19:53:20.283' AS DateTime), N'dočasné pozastavení série: programujeme na WP 8.1', N'<p>Dnešní díl (jestli se to vůbec může nazvat “díl”), se nebude týkat programování, ale -jak vychází na jevo z popisku- vysvětlení dlouhé pauzy a dočasného pozastavení série. První věc je (jak už jsem psal do komentáře), že poslední 2 měsíce byly neskutečně nabité. Setkalo se hodně věcí, které se museli řešit bez prodleně, věci s větší prioritou, kvůli kterým jsem se nedostal k napsání pokračování. Když jsem zrovna neřešil školu, MSP program, zákazníky, nebo administrativu okolo firmy, neměl jsem prostě dostatek sil (nemluvě o náladě), k napsání odborného článku, nebo spíš další kapitolu učebnice.  <p>Uvědomuji si, že jsem čtenáře nevnadil na něco a svůj slib nedodržel, ale obávám se, že jsem si na začátku vzal moc velké sousto, které nejsem schopen unést. Nechci pokračovat v něčem, kde je velká pravděpodobnost, že selžu. Radši zklamu určitý počet lidí teď, než později ještě více. Jednou sérii určitě obnovím, možná spíš začnu úplně od začátku (představoval bych si, že s příchodem Windows Phone 10).  <p>Tímto bych chtěl tedy poděkovat, za krásná čísla, které se pod mými články ukázali, za odezvu v komentářích a podporu obecně. Zároveň, to ale nesmíte brát tak, že končím s psaním úplně. Dokonce se ani tolik neodtrhnu od nynější (již tedy pozastavené série vývoje na WP 8.1), jen budu psát v jiné formě. Rozhodl jsem se ze začátku přistoupit na strategii kamaráda a kolegy <a href=""http://kasik96.tumblr.com/"">Martina Kaše</a>.  <p>Tzv. strategie se zakládá na principu “zápiskách”. Funguje tak, že narazím na problém, novou věc, novou metodiku apod. a popisují do článku, tak aby posloužila jako názorný příklad, návod pro další vývojáře, kteří s tím již tak nebudou muset zápasit a složitě hledat řešení v problémech ostatních, v angličtině a podobně.  <p>To by jako vysvětlení bylo snad vše. Doufám, že to se mnou nevzdáte a budeme se nadále potkávat v dalších publikacích.  <p>Na konec bych rád znovu konstatoval, abyste se nebáli se mě na cokoliv zeptat. Ať zde v komentářích, nebo kdekoliv jinde.  <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/55c8b33166ed_7DEB/clip_image001%5B4%5D.png""><img title=""clip_image001[4]"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""clip_image001[4]"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/55c8b33166ed_7DEB/clip_image001%5B4%5D_thumb.png"" width=""23"" height=""23""></a>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8436, 19, CAST(N'2015-01-31T22:55:47.567' AS DateTime), N'MonoGame–klávesnice pod kontrolou', N'<p>Vítejte podruhé, posledně jsme se podávali na to, co to vlastně MonoGame je a v tomto díle budeme pokračovat. Máme modrou plochu na které je vykreslené logo MonoGame a to se pokusíme v tomto díle rozpohybovat pomocí klávesnice. Je to jeden z nejčastějších úkolů, které budeme při dělání her potřebovat.  <h2>Klávesnice a myš</h2> <p>Pro práci s klávesnicí a myší máme v MonoGame stejně jako v XNA k dispozici třídy <em>Mouse</em> a <em>Keyboard</em>. S jejich pomocí lze za použití metod <em>GetState</em> získat současný stav klávesnice a myši. U klávesnice máme k dispozici všechny stisklé klávesy a u myši navíc její polohu. Tento přístup nám ale při dělání složitějších her stačit nebude. Proč? Není totiž možné zjistit, jestli tlačítko myši bylo právě nyní stisknuto nebo právě nyní puštěno, stejně tak u klávesnice. Ale tento nedostatek si odstraníme s využitím vlastní pomocné třídy.  <p>Začneme klávesnicí pomocí které rozpohybujeme logo MonoGame. Vytvoříme si novou privátní proměnnou kam budeme ukládat stav klávesnice:</p><pre class=""brush:csharp"">KeyboardState keyboard;</pre>
<p>Pokud nemáte v projektu příslušný jmenný prostor, tak si jej přidejte:</p><pre class=""brush:csharp"">using Microsoft.Xna.Framework.Input;</pre>
<p>Dále budeme potřebovat proměnnou s pozicí našeho obrázku:</p><pre class=""brush:csharp"">Vector2 position;</pre>
<p>A tu si hned v metodě <em>Initialize</em> nastavíme na výchozí polohu:</p><pre class=""brush:csharp"">position=new Vector2(100,100);</pre>
<p>V metodě Update si získáme stav klávesnice zavoláním metody <em>GetState</em>:</p><pre class=""brush:csharp"">keyboard=Keyboard.GetState();</pre>
<p>a ten budeme dále používat. V této struktuře máme připraveny i pomocné metody, které si použijeme v podmínce. V případě stisknutí pravé šipky na klávesnici chceme aby se obrázek posunul po ose X o jedničku.</p><pre class=""brush:csharp"">if(keyboard.IsKeyDown(Keys.Right)){
  position.X+=1;
}</pre>
<p>A v případě stisknutí levé šipky chceme aby se posouval zase zpět:</p><pre class=""brush:csharp"">if(keyboard.IsKeyDown(Keys.Left)){
  position.X-=1;
}</pre>
<p>Nesmíme si také zapomenout přepsat pozici pro vykreslení:</p><pre class=""brush:csharp"">spriteBatch.Draw(texture,position,Color.White);</pre>
<p>A máme hotovo. Pokud si nyní hru spustíme tak můžeme pěkně hýbat logem MonoGame.</p>
<p>Ale jak jsem říkal výše není to zrovna všeobecné řešení, pro další pokračování toho budeme potřebovat zjistit více. Toho dosáhneme snadno, stačí nám si uchovat předešlý stav klávesnice a myši a porovnat jej se současným. Takže když budeme chtít zjistit jestli byla klávesa stisknuta právě nyní tak musí platit, že v předešlém stavu je klávesa puštěna a v tomto je stisknuta. Ideální na to bude když si na to napíšeme pomocnou třídu. Nazveme ji třebas Input a bude celá statická. Přidáme si dvě proměnné pro starý stav klávesnice a pro současný stav klávesnice:</p><pre class=""brush:csharp"">private static KeyboardState oldKeyboard;
private static KeyboardState keyboard;</pre>
<p>Dále potřebujeme metodu, pomocí které oba stavy budeme updatovat, já bych jí nazval <em>Update</em>, ať se držíme zavedených názvů. Uložíme si nejprve starý stav klávesnice a zjistíme si nový:</p><pre class=""brush:csharp"">public static void Update(){
  oldKeyboard = keyboard;
  keyboard = Keyboard.GetState();
}</pre>
<p>A dále napíšeme tři pomocné metody. Máme totiž celkem čtyři možné stavy, které můžeme chtít detekovat:</p>
<ul>
<li>
<p>klávesa nebyla nikdy stištěna vůbec (v obou stavech je puštěna)</p>
<li>
<p>klávesa byla právě nyní stištěna (v předchozím je puštěna a v tomto je stisknuta)</p>
<li>
<p>klávesa je držena (v obou stavech je klávesa stisknuta)</p>
<li>
<p>klávesa byla právě nyní puštěna (v předchozím stavu je stištěna a v tomto puštěna)</p></li></ul>
<p>Jejich implementace je pak už velmi snadná a tak jen telegraficky všechny metody:</p><pre class=""brush:csharp"">public static bool IsKeyUp(Keys key){
  return oldKeyboard.IsKeyUp(key) &amp;&amp; keyboard.IsKeyUp(key);
}

public static bool IsKeyPressed(Keys key){
  return oldKeyboard.IsKeyUp(key) &amp;&amp; keyboard.IsKeyDown(key);
}

public static bool IsKeyHold(Keys key){
  return oldKeyboard.IsKeyDown(key) &amp;&amp; keyboard.IsKeyDown(key);
}

public static bool IsKeyReleased(Keys key){
  return oldKeyboard.IsKeyDown(key) &amp;&amp; keyboard.IsKeyUp(key);
}
</pre>
<p>Jediné co nyní potřebujeme, aby možné třídu využívat, je zavolat její metodu Update v herní smyčce. A pak už jen využívat její služby, takže si předěláme posun obrázku na tuto třídu, řekněme, že budeme chtít při stisknutí šipky posunout obrázek jen jednou o 10 jednotek. Takže si to zkusíme, je to snadné:</p><pre class=""brush:csharp"">if (Input.IsKeyPressed(Keys.Left)){
  position.X -= 10;
}
if (Input.IsKeyPressed(Keys.Right)){
  position.X += 10;
}</pre>
<p>A máme hotovo. Pokud si nyní hru spustíme zjistíme, že při každém stisku nám obrázek poskočí. A to by mohlo myslím pro dnešek stačit. Příště se podíváme na to, jak ochočit myš. A taky bych si naplánoval nějakou snadnou hru, kterou si zkusíme napsat, než se vrhneme nějako na to 3D. Celý projekt si můžete stáhnout zde:
<div id=""scid:6E335E69-603A-4F1E-AA4B-726D584133B4:e546e679-3203-4a62-a023-9ce91bb027f9"" class=""wlWriterSmartContent"" style=""float: none; padding-bottom: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline; padding-right: 0px""><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/f2ed04500881_11560/c1afe8d6-8725-4999-b649-e2754598359d_MG02.zip"">MG02.zip</a></div>. Budu se těšit na vaše komentáře.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8443, 3, CAST(N'2015-03-22T21:22:23.587' AS DateTime), N'Programátorská hádanka: Doplňte chybějící funkci', N'<p>Mám následující app.config:</p> <pre class=""brush: xml"">
&lt;configuration&gt;
    &lt;appSettings&gt;
        &lt;add key=""Domain"" value=""google.cz"" /&gt;
        &lt;add key=""Port"" value=""80"" /&gt;
        &lt;add key=""Name"" value=""Tomas"" /&gt;
    &lt;/appSettings&gt;
&lt;/configuration&gt;
</pre>
<p>A mám následující program:</>
<pre class=""brush: csharp"">
var settings = new ApplicationSettings(ConfigurationManager.AppSettings);
Console.WriteLine(settings.Domain);
Console.WriteLine(settings.Port);
Console.WriteLine(settings.Name);
</pre>
<p>Tento program na konzoli vypíše hodnoty z konfiguračního souboru. Potud jasné.</p>
<p>&nbsp;</p>
<p>A teď máme třídu <strong>ApplicationSettings</strong>, která vypadá takto:</p>
<p>&nbsp;</p><pre class=""brush: csharp"">    public class ApplicationSettings
    {
        private readonly NameValueCollection appSettingsCollection;

        public ApplicationSettings(NameValueCollection appSettingsCollection)
        {
            this.appSettingsCollection = appSettingsCollection;
        }

        // TODO: Sem doplňte funkci GetValue, která vytáhne příslušnou hodnotu z kolekce
        // Tato funkce se ale musí volat bez jakéhokoliv parametru
        

        public string Domain => GetValue();
        public string Port => GetValue();
        public string Name => GetValue();

    }
</pre>
<p>Jak musí vypadat funkce <strong>GetValue</strong>, aby to fungovalo správně? Podstatné je, že <strong>nesmíte změnit to, jak vypadají vlastnosti Domain, Port a Name - musí i nadále volat tu funkci GetValue bez parametru</strong>.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8445, 3, CAST(N'2015-03-24T17:49:42.517' AS DateTime), N'Uvedena nová služba Azure App Service', N'<p>Dnes <a href=""http://weblogs.asp.net/scottgu/announcing-the-new-azure-app-service"">Scott Guthrie</a> na svém blogu oznámil uvolnění nové služby <strong>Azure App Service</strong>. Jedná se o službu, která umožňuje spojit dohromady webové aplikace, mobilní služby, aplikační logiku a API, nasazovat ji, škálovat a provozovat jako jeden celek.</p> <h3>Webové aplikace (Web Apps)</h3> <p>Webové aplikace v rámci <strong>Azure App Service </strong>mají všechny funkce, které najdete u klasických <strong>Azure Websites</strong>.</p> <ul> <li>Podpora .NET, Node.js, Java, PHP a Pythonu</li> <li>Podpora auto-scale</li> <li>Publish z Visual Studia, pomocí FTP a ze source control systému (continuous delivery)</li> <li>Podpora virtuálních sítí a napojení na on-premise prostředí</li> <li>Nasazení na více deployment slotů – staging, production atd.</li> <li>Podpora pro WebJobs pro dlouhotrvající úlohy na pozadí</li></ul> <p>Zároveň dochází k přejmenování <strong>Azure Websites</strong> na <strong>Azure Web Apps</strong>. Podmínky a ceny zůstávají nezměněny.</p> <h3>Mobilní aplikace (Mobile Apps)</h3> <p>Mobilní aplikace jsou rozšířením <strong>Azure Mobile Services</strong>.</p> <ul> <li>Podpora auto-scale</li> <li>Podpora služby Traffic Manager pro geografické škálování služeb</li> <li>Nasazení ze source control systému (continuous delivery)</li> <ul> <li>Podpora virtuálních sítí a napojení na on-premise prostředí  <li>Nasazení na více deployment slotů – staging, production atd.  <li>Podpora pro WebJobs pro dlouhotrvající úlohy na pozadí</li></ul></ul> <p>Novinkou je, že mobilní službu můžete nasadit společně s <strong>Web App </strong>společně a platit tak podstatně méně.</p> <h3>Aplikační logika (Logic Apps)</h3> <p><strong>Logic Apps</strong> jsou úplně novou službou, která umožňuje definovat workflow pro automatizaci business procesů. Tato služba umožňuje “naklikat” proces a využít connectory, které se umí napojit na různé služby, například Dynamics CRM, Salesforce, sociální sítě, SMS bránu, Office 365 a mnoho dalších služeb.</p> <p><img alt=""image"" src=""https://mscblogs.blob.core.windows.net/media/scottgu/WindowsLiveWriter/AnnouncingthenewAzureAppService_122D1/image_thumb_6.png"" width=""563"" height=""293""></p> <p><font size=""1"">(Obrázek byl převzat z </font><a href=""http://weblogs.asp.net/scottgu/announcing-the-new-azure-app-service""><font size=""1"">blogu Scotta Guthrieho</font></a><font size=""1"">)</font></p> <p>Seznam všech služeb, na které se můžete napojit, je <a href=""https://mscblogs.blob.core.windows.net/media/scottgu/WindowsLiveWriter/AnnouncingthenewAzureAppService_122D1/image_thumb.png"">zde</a>.</p> <h3>API (API Apps)</h3> <p>Tato služba umožňuje rozšířit existující API (vytvořené například pomocí ASP.NET Web API nebo jiné technologie) a přidává například možnost správy uživatelských oprávnění, vygenerování SDK pro různé platformy a zaintegrování s <strong>Logic Apps</strong>.</p> <p>&nbsp;</p> <p>Cenová politika je <a href=""http://azure.microsoft.com/en-us/pricing/details/app-service/"">velmi příznivá</a>, v zásadě vychází z cenového modelu <strong>Azure Websites</strong>. Pro velmi malé aplikace můžete využít jak variantu Free, která má různá omezení, pro větší aplikace můžete využít několik Standard instancí. Výhodou je především to, že různé typy aplikací (například Web App a Mobile App) lze dostat na jednu instanci a snížit tím náklady.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8446, 15, CAST(N'2015-04-06T17:30:00.000' AS DateTime), N'VS2015 CTP 6 - Chyba přidávání NuGet balíčků do projektu typu ASP.NET 5 (vNext)', N'<p><u>Problém:</u>  <p>VS 2015 CTP 6 po instalaci neumí stáhnout žádný NuGet package (tj. v důsledku toho nainstalovat jiný NuGet package, než který je již po instalaci v “cache” adresáři <i>C:\Users\&lt;user&gt;\.k\packages</i>). V Package Manager Logu se toto pozná tak, že při instalaci balíčku vybíhá chyba například:  <blockquote> <p><font color=""#400080"">Error: FindPackagesById: System.Threading.Tasks Response status code does not indicate success: 400 (One of the request inputs is out of range.).</font></p></blockquote> <p><u>Řešení:</u>  <p>Je to “known bug”, způsobený tím, že <em>kpm</em> ještě neumí nové NuGet 3 API, ale VS 2015 CTP 6 při instalaci již přepne zdroj na nový NuGet feed. Je potřeba ve VS v <i>Options</i> přenastavit <i>Available package sources</i> z nového:  <p><i>https://api.nuget.org/v3/index.json</i> <p>zpět na starší:  <p><i>https://www.nuget.org/api/v2/</i> <p>Takto je správné nastavení:  <p><img title=""NuGetPackageSourceVS2015CTP6"" style=""display: inline"" alt=""NuGetPackageSourceVS2015CTP6"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/e14f9c9c85c9_10ECD/NuGetPackageSourceVS2015CTP6_e547468a-da96-4f01-957b-e374592d656c.png"" width=""620"" height=""416"">  <p>Intelisence při editaci souboru <i>project.json</i> mi ale nezačalo korektně fungovat ani potom, to bude asi druhá nesouvisející chyba.  <p>Zdroj:  <p><a href=""http://stackoverflow.com/questions/28842138/vs-2015-ctp-6-nuget-package-source"" target=""_blank"">http://stackoverflow.com/questions/28842138/vs-2015-ctp-6-nuget-package-source</a><br><a href=""http://blog.nuget.org/20150226/nuget-3.0-beta2.html"" target=""_blank"">http://blog.nuget.org/20150226/nuget-3.0-beta2.html</a>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8447, 3, CAST(N'2015-04-12T17:17:59.293' AS DateTime), N'Jak vyresetovat Experimentální instanci Visual Studia', N'<p>Při vývoji <a href=""http://github.org/riganti/redwood"">Redwoodu</a> jsem narazil na situaci, kdy experimentální instance Visual Studia začne hned po startu padat na NullReferenceException. Stává se to náhodně a nepodařilo se mi najít příčinu.</p> <p>Na <a href=""https://www.youtube.com/watch?v=TwIDQxLyT9k"">přednášce</a> se mě na to kdosi ptal, tak tady je návod.</p> <p>&nbsp;</p> <p>Každopádně pomáhají na to dva kroky:</p> <ol> <li>Z nabídky Start pustit skript “Reset the Visual Studio Next Experimental Instance”, který tam instalátor VS přidá.</li> <li>Spustit regedit a smazat následující klíče:<br><br>HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\10.0Exp<br>HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\10.0Exp_Config</li></ol>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8448, 15, CAST(N'2015-04-17T15:22:52.597' AS DateTime), N'Přihlášení pomoci Windows Authentizace', N'<p>Pro reprezentaci Windows identity nám v .NET slouží objekt <a href=""https://msdn.microsoft.com/en-us/library/system.security.principal.windowsidentity(v=vs.110).aspx"" target=""_blank""><strong>WindowsIdentity</strong></a>. Pomoci něj se například můžeme dostat k aktuální autentizované identitě ve windows pomoci volání: (interně se použije current thread token)</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();</pre>
<p>Nás ale v tomto článku bude zajímat jiný scénář. Někdy může být v aplikaci vyžadováno ověření <strong>Windows uživatelským účtem, který je jiný než ten, pod kterým je zrovna uživatel na počítači přihlášen</strong>. Nebo se může jednat o webovou ASP.NET aplikaci, kde k požadované Windows identitě uživatele nemusíme mít přístup (například když aplikace používá Anonymous a Forms Authentication).</p>
<p>Pro přihlášení pomoci přihlašovacího jména a hesla Windows slouží Win32 API funkce <strong><a href=""https://msdn.microsoft.com/en-us/library/windows/desktop/aa378184(v=vs.85).aspx"" target=""_blank"">LogonUser</a></strong>. Překvapivé je to, že v .NETu nemáme pro její volání žádnou wrapper metodu (jedno volání je ve WIF v <em>System.IdentityModel.Tokens.WindowsUserNameSecurityTokenHandler </em>metodě <em>ValidateToken</em>, tam ale není možné předat parametry, které můžeme potřebovat). Nezbývá tedy nic jiného, než použít přímo tuto API funkci a wrapper pro ní si napsat sami. Já jsem implementaci umístil do samostatné třídy <strong>WindowsAuthenticationManager</strong> a v ní do metody <strong>LogonWindowsUser</strong>.</p>
<p>Než si třídu ukážeme, všimněte si ještě parametrů, které se na vstupu funkci <em>LogonUser</em> předávají:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">bool LogonUser([In] string lpszUserName, [In] string lpszDomain, [In] string lpszPassword, [In] uint dwLogonType, [In] uint dwLogonProvider, out SafeCloseHandle phToken)</pre>
<p><br>Budou nás teď zajímat parametry <em><strong>lpszUserName</strong></em> a <em><strong>lpszDomain</strong></em>. Parametr <em><strong>lpszUserName</strong></em> může být buď:</p>
<ul>
<li>Přihlašovací jméno uživatelského účtu nazývané <strong>SAM account name</strong> (přihlašovací jméno pro systémy starší než Windows 2000), pak parametrem <strong><em>lpszDomain</em></strong> určíme doménu uživatelského účtu. Pokud pro <strong>SAM account name</strong> doménu neurčíme, použije se výchozí, pokud jako doménu zadáme “.”, použije se lokální databáze účtů (doména počítače). 
<li>Druhou možností je parametr <em><strong>lpszUserName</strong></em> předat ve formátu <strong>User Principal Name</strong> <strong>(UPN)</strong>, jedná se o formát ve tvaru <em>User@DNSDomainName</em> a v takovém případě již parametr <strong><em>lpszDomain</em></strong> nezadáváme.<br><em>DNSDomainName</em> přitom nemusí odpovídat jménu domény, jména můžou být různá. Nejčastěji se nastavuje tak, aby bylo stejné jako má uživatel emailovou adresu, např. můj účet <em>jan.holan</em> v doméně <em>netdomain.local</em> má UPN <em>jan.holan@h2net.cz</em>.</li></ul>
<p>Protože je zvykem, že uživatel zadává své přihlašovací jméno do jednoho pole dohromady s doménou ve tvaru <em>Doména\Přihlašovací jméno</em>, zavedeme kromě metody <em>LogonWindowsUser(string user, string domain, string password, WindowsAuthenticationLogonType logonType)</em> i variantu pouze s parametry <em>LogonWindowsUser(string userName, string password, WindowsAuthenticationLogonType logonType)</em>, ve které doménu ze zadaného přihlašovacího jména extrahujeme.</p>
<p>Kompletní třída <strong>WindowsAuthenticationManager</strong> a obě varianty metody <strong>LogonWindowsUser</strong> vypadají takto:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;
using System.Security.Claims;

namespace WindowsAuthentication
{
    #region SafeCloseHandle class
    internal sealed class SafeCloseHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCloseHandle() : base(true)
        {
        }

        internal SafeCloseHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport(""kernel32.dll"", SetLastError = true, ExactSpelling = true)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(base.handle);
        }
    }
    #endregion

    public enum WindowsAuthenticationLogonType
    {
        Interactive = 2,
        Network = 3,
        Batch = 4,
        Service = 5,
        Unlock = 7,
        NetworkCleartext = 8,
        NewCredentials = 9
    }

    public static class WindowsAuthenticationManager
    {
        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport(""advapi32.dll"", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool LogonUser([In] string lpszUserName, [In] string lpszDomain, [In] string lpszPassword, [In] uint dwLogonType, [In] uint dwLogonProvider, out SafeCloseHandle phToken);
        }

        private const uint LOGON32_PROVIDER_DEFAULT = 0;
        private const uint LOGON32_PROVIDER_WINNT40 = 2;    //NTLM
        private const uint LOGON32_PROVIDER_WINNT50 = 3;    //Negotiate (NTLM, Kerberos or other SSP (Security Support Provider))
        private const uint LOGON32_LOGON_INTERACTIVE = 2;
        private const uint LOGON32_LOGON_NETWORK = 3;
        private const uint LOGON32_LOGON_BATCH = 4;
        private const uint LOGON32_LOGON_SERVICE = 5;
        private const uint LOGON32_LOGON_UNLOCK = 7;
        private const uint LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        private const uint LOGON32_LOGON_NEW_CREDENTIALS = 9;

        #region action methods
        public static WindowsPrincipal LogonWindowsUser(string userName, string password, WindowsAuthenticationLogonType logonType = WindowsAuthenticationLogonType.Interactive)
        {
            //userName can be in form ''Domain\SAMAccountName'' or only SAMAccountName or UserPrincipalName (User@DNSDomainName)
            //Extract domain if userName is in form ''Domain\SAMAccountName'', domain can be ''.'' for local accounts
            string domain = null;
            char[] separator = new char[] { ''\\'' };
            string[] userNameParts = userName.Split(separator);
            if (userNameParts.Length != 1)
            {
                if (userNameParts.Length != 2 || string.IsNullOrEmpty(userNameParts[0]))
                {
                    throw new ArgumentException(""The username format is not valid. The username format must be in the form of ''username'' or ''domain\\username''."", ""userName"");
                }
                userName = userNameParts[1];
                domain = userNameParts[0];
            }

            return LogonWindowsUser(userName, domain, password, logonType);
        }

        public static WindowsPrincipal LogonWindowsUser(string user, string domain, string password, WindowsAuthenticationLogonType logonType = WindowsAuthenticationLogonType.Interactive)
        {
            //Code base on System.IdentityModel.Tokens.WindowsUserNameSecurityTokenHandler ValidateToken
            SafeCloseHandle userToken = null;
            try
            {
                if (!NativeMethods.LogonUser(user, domain, password, (uint)logonType, LOGON32_PROVIDER_DEFAULT, out userToken))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(error);
                }

                var windowsIdentity = new WindowsIdentity(userToken.DangerousGetHandle(), ""Password"", WindowsAccountType.Normal, true);
                windowsIdentity.AddClaim(new Claim(""http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant"", System.Xml.XmlConvert.ToString(DateTime.UtcNow, ""yyyy-MM-ddTHH:mm:ss.fffZ""), ""http://www.w3.org/2001/XMLSchema#dateTime""));
                windowsIdentity.AddClaim(new Claim(""http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod"", ""http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/password""));

                return new WindowsPrincipal(windowsIdentity);
            }
            finally
            {
                if (userToken != null)
                {
                    userToken.Close();
                }
            }
        }
        #endregion
    }
}</pre>
<p>Metoda <strong>LogonWindowsUser</strong> kromě zavolání funkce LogonUser dále použije získaný handler a vytvoří objekt <strong>WindowsIdentity</strong>. Ten jak asi víte v .NET 4.5 již používá claims, a to nám umožňuje si do něj přidat ještě libovolné vlastní hodnoty.</p>
<p>Následující příklad volání autentizace uživatele ještě dále ukazuje přidání do identity celého jména z AD atributů vytaženého pomoci <em>System.DirectoryServices.AccountManagement</em>:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">public static class AuthenticationManager
{
    #region action methods
    public static WindowsPrincipal AuthenticateWindowsUser(string userName, string password, WindowsAuthenticationLogonType logonType = WindowsAuthenticationLogonType.Interactive)
    {
        var principal = WindowsAuthenticationManager.LogonWindowsUser(userName, password, logonType);
        var identity = (WindowsIdentity)principal.Identity;

        //Get Windows user DisplayName and add it to Claims
        string displayName = GetUserDisplayName(identity.Name, identity.User);  //In identity.Name is NTAccountName (''Domain\SAMAccountName''), in identity.User is user SID
        identity.AddClaim(new Claim(""DisplayName"", displayName));

        return new WindowsPrincipal(identity);
    }
    #endregion

    #region private member functions
    private static string GetUserDisplayName(string NTAccountName, SecurityIdentifier userSID)
    {
        using (var context = GetPrincipalContext(NTAccountName))
        {
            using (var userPrincipal = UserPrincipal.FindByIdentity(context, userSID.ToString()))
            {
                if (userPrincipal != null)
                {
                    return string.IsNullOrEmpty(userPrincipal.DisplayName) ? userPrincipal.Name : userPrincipal.DisplayName;
                }
            }
        }

        return null;
    }

    private static PrincipalContext GetPrincipalContext(string NTAccountName)
    {
        string domain = ExtractDomain(NTAccountName);

        ContextType type = ContextType.Domain;
        if (Environment.MachineName.Equals(domain, StringComparison.OrdinalIgnoreCase))
        {
            type = ContextType.Machine;
        }

        return new PrincipalContext(type, domain);
    }

    private static string ExtractDomain(string NTAccountName)
    {
        int index = NTAccountName.IndexOf(''\\'');
        if (index != -1)
        {
            return NTAccountName.Substring(0, index);
        }

        return null;
    }
    #endregion
}</pre>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8449, 3, CAST(N'2015-04-17T23:48:58.423' AS DateTime), N'MonoGame a Mouse.SetPosition ve Windows Store aplikaci', N'<p><a href=""http://www.monogame.net/"">MonoGame</a> je knihovna, která vznikla jako nástupce XNA a jejímž cílem je umožnit vývoj her pro všechny platformy. Pomocí Xamarinu lze kompilovat pro Android a iOS a lze v ní vyvíjet jak klasické okenní Windows hry, tak i Windows Store aplikace.</p> <p>API této knihovny je až na pár detailů stejné jako u XNA, takže teoreticky stačí hru překompilovat oproti jiným knihovnám – názvy tříd, namespaců a funkcí jsou stejné. V praxi ovšem narazíte na mnoho drobných rozdílů daných omezeními jednotlivých platforem.</p> <p>&nbsp;</p> <p>Tak například funkce <strong>Mouse.SetPosition</strong>, která má za úkol nastavit kurzor myši na danou souřadnici na obrazovce. Používá se typicky v situaci, kdy píšete hru s first person kamerou a myš používá uživatel pro míření na cíl. Kurzor myši potřebujete mít neustále uprostřed obrazovky a jakmile se pohne, tak jen posunete kameru, ale kurzor vrátíte zase zpět.</p> <p>Potíž je v tom, že ve WinRT nemůže aplikace pozici myši měnit. Naštěstí je ale možné myš přepnout do tzv. relativního módu, kdy je kurzor skrytý a v aplikaci dostáváte události, že se myš pohnula, přičemž se neberou v úvahu okraje obrazovky, takže můžete jet libovolným směrem “donekonečna”. Tento mód je určen pro hry, takže pokud chcete first person kameru implementovat v MonoGame ve Windows Store aplikaci, je potřeba použít následující postup.</p> <p>&nbsp;</p> <p>Nejprve je nutné skrýt kurzor myši a zaregistrovat odběr události <strong>MouseMoved</strong>. </p><pre class=""brush: csharp"">private Vector2 lastMouseMovement;

public Game1()
{

    ...

    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = null;
    Windows.Devices.Input.MouseDevice.GetForCurrentView().MouseMoved += (sender, args) =&gt;
    {
        lastMouseMovement = new Vector2(args.MouseDelta.X, args.MouseDelta.Y);
    };
}
</pre>
<p>Ve chvíli, kdy se pozice myši změní, bude naše událost zavolána, a do proměnné lastMouseMovement si uložíme hodnoty, o které se myš posunula.</p>
<p>Ve funkci <strong>Update</strong> pak stačí na tyto hodnoty zareagovat (a proměnnou vynulovat, abyste ji nepoužili opakovaně).</p>
<p><em>Pokud píšete hru, asi nikdy nebudete potřebovat tento režim vypnout (platí jen pro vaši aplikaci - vypne se, jakmile uživatel aplikaci opustí nebo ukončí). Nicméně pokud byste potřebovali tento režim vypnout (nedají se v něm pomocí myši otevřít systémové boční panely), tak stačí do vlastnosti <strong>PointerCursor</strong> vrátit zpět to, co v ní bylo, a odebrat handler na danou událost.</em></p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8450, 3, CAST(N'2015-04-20T16:03:42.380' AS DateTime), N'Chyba “Object reference not set to an instance of an object” při deployi Azure Cloud Service', N'<p>Dnes jsem potřeboval nasadit jednu starší Cloud Service a při deploymentu se objevila následující chyba. </p> <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/2c1cffbf5734_E08E/image_2.png""><img title=""image"" style=""border-top: 0px; border-right: 0px; background-image: none; border-bottom: 0px; padding-top: 0px; padding-left: 0px; border-left: 0px; display: inline; padding-right: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/2c1cffbf5734_E08E/image_thumb.png"" width=""684"" height=""140""></a></p> <p>Hledání na Googlu směřovalo hlavně k prověření faktu, jestli jsou nastavené správné certifikáty v portálu a v projektu, což jsem několikrát kontroloval, dokonce jsem zkusil vygenerovat nové, ale nepomohlo to.</p> <p>Nakonec jsem zjistil, že při upgradu z Azure SDK 2.4 na 2.5 Visual Studio někdy nesprávně zmigruje soubor <strong>diagnostics.wadcfgx</strong>. Řešením je příslušné soubory smazat, kliknout na každou roli pravým tlačítkem a vybrat <strong>Add Diagnostics Configuration</strong>, čímž se vytvoří nová defaultní a ve správném formátu.</p> <p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/2c1cffbf5734_E08E/image_4.png""><img title=""image"" style=""border-top: 0px; border-right: 0px; background-image: none; border-bottom: 0px; padding-top: 0px; padding-left: 0px; border-left: 0px; display: inline; padding-right: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/2c1cffbf5734_E08E/image_thumb_1.png"" width=""247"" height=""124""></a></p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8451, 16, CAST(N'2015-05-06T23:20:40.300' AS DateTime), N'Canopy – přehledné UI testy', N'<p>V poslední době jsem se hodně věnoval různým formám automatického testování, postupně jsem se prokousal od unit testů, přes integrační testy až po UI testy. Zkoušel jsem CodedUI, Selenium a díky kolegovy jsem narazil na Canopy. </p> <p>O UI testování se často říká že je složité, zbytečné, drahé a nevyplatí se. V tomhle článku bych vás chtěl přesvědčit o opaku. Ukážeme si že se dají psát UI testy i jednoduše a srozumitelně. Napíšeme si jednoduché smoke testy pro již běžící web. </p> <p>Canopy je UI test Framework postavený nad Seleniem, díky tomu má dobrou podporu a široké možnosti. Je napsaný v F# a jeho hlavním cílem je srozumitelná syntaxe testů.</p><pre class=""brush: csharp"">""click polling"" &amp;&amp;&amp; fun _ -&gt; 
    url ""http://lefthandedgoat.github.io/canopy/testpages/autocomplete""
    click ""#search""
    click ""table tr td""
    ""#console"" == ""worked""
</pre>
<p>Pojďme se na to podívat. Pokud neumíte F#, tak nevadí, elementární znalosti vám budou stačit, alespoň máte možnost naučit se něco nového. Syntaxi F# budu přirovnávat k C#. </p>
<h2>První test</h2>
<p>Založíme nový projekt F# Console app. Canopy potřebuje .Net 4 nebo vyšší.</p>
<p>.<a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_4.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_thumb_1.png"" width=""640"" height=""442""></a></p>
<p>Přidáme nuget package Canopy jako dependecy se nám nainstalujte i Selenium.<a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/01_NugetPackageInstall_2.png""><img title=""01_NugetPackageInstall"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""01_NugetPackageInstall"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/01_NugetPackageInstall_thumb.png"" width=""640"" height=""386""></a></p>
<p>Stávající kód nahradíme naším prvním testem</p><pre class=""brush: csharp"">open canopy
open runner
open System
 
//První UI test
""test method"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://google.com""  //Přejdi na URL
 
start firefox //Nastartuje browser
run() //Spustí všechny UI testy
</pre>
<p>open System je obdoba C# using System. ""test method"" &amp;&amp;&amp; fun _ –&gt; je deklarace testu kde “Test method” je název našeho testu a na řádku pod je implementace testu. I tak málo nám stačí ke zprovoznění UI testů. </p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_11.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_thumb_4.png"" width=""640"" height=""344""></a></p>
<p>Pozor! F# podobně jako Python používá bílé znaky k ukončení bloku. </p><pre class=""brush: csharp"">""test method"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://google.com""
    //Je to až v metodě, tudíže se to nikdy nepustí
    start firefox 
    run()
</pre>
<p>Ovšem tenhle test zatím nic netestuje, pouze nám otevře Firefox a načte stránku. Pojďme si něco vyhledat a otestovat. Testování provádíme klasicky pomocí assertů, assert je ověření zda platí daná podmínka, Canopy má celou řadu assertů, jejich kompletní seznam najdete v <a href=""http://lefthandedgoat.github.io/canopy/assertions.html"">dokumentaci</a>.</p><pre class=""brush: csharp"">""test google search for Canopy home page"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://google.com""
    ""#lst-ib"" &lt;&lt; ""Canopy UI test framework"" //do inputu s id lst-ib vložíme string ""Canopy UI test framework""
    press enter //simulujeme stisk enter
    click "".srg .g a"" //kliknutí na první výsledek hledání. Selektor je poněkud kostrbatý 
    on ""http://lefthandedgoat.github.io/canopy/"" //Assert je aktuální stránka 
start firefox
run()
</pre>
<p>Nyní test testuje, jestli po kliknutí na první odkaz vyhledávání je jako první výsledek domovská stránka Canopy. Poslední věc, kterou od testu očekáváme je, že po sobě uklidí. Na to máme metodu quit(). Výsledný test tedy vypadá takto.</p><pre class=""brush: csharp"">open canopy
open runner
open System
 
""test google search for Canopy home page"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://google.com""
    ""#lst-ib"" &lt;&lt; ""Canopy UI test framework""
    press enter
    click "".srg .g a""
    on ""http://lefthandedgoat.github.io/canopy/""
 
start firefox
run()
quit()
</pre>
<h2>Když jeden prohlížeč nestačí</h2>
<p>Testovat vše jen ve Firefoxu by bylo trochu málo, Selenium podporuje celou řadu prohlížečů, aktuální seznam najdete <a href=""http://docs.seleniumhq.org/about/platforms.jsp"">zde</a>. Pojďme si pustit náš test i v Chrome.</p>
<p>K tomu potřebujeme <a href=""https://sites.google.com/a/chromium.org/chromedriver/"">ChromeDriver</a>. Canopy defaultně očekává chrome driver na cestě C:\\ChromeDriver.exe, to ale není moc vhodné. My si ho nainstalujeme přes nuget, najdeme ho jako Selenium.WebDriver.ChromeDriver</p>
<p>.<a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/02_NugetPackageInstallChrome_2.png""><img title=""02_NugetPackageInstallChrome"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""02_NugetPackageInstallChrome"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/02_NugetPackageInstallChrome_thumb.png"" width=""640"" height=""386""></a></p>
<p>Do solution se nám přidá soubor chromedriver.exe. Zkontrolujte si v properties souboru, že je nastaveno Copy if newer v Copy to output directory. </p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_15.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_thumb_6.png"" width=""240"" height=""132""></a></p>
<p>Cestu k driveru přenastavíme pomocí následujících dvou řádků, jak asi tušíte operátor &lt;- je v F# operátorem přiřazení. ""."" označuje aktuální složku.</p><pre class=""brush: csharp"">open configuration

chromeDir &lt;- "".""
</pre>
<p>Teď již můžeme testy pustit i pro chrome</p><pre class=""brush: csharp"">open canopy
open runner
open System
open configuration

chromeDir &lt;- "".""

""test google search for Canopy home page"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://google.com""
    ""#lst-ib"" &lt;&lt; ""Canopy UI test framework""
    press enter
    click "".srg .g a""
    on ""http://lefthandedgoat.github.io/canopy/""

start chrome
run()

start firefox
run()

quit()
</pre>
<p>Na gitu je již přidaná metoda runFor, díky které půjde spouštět testy paralelně v několika prohlížečích najednou, ale v aktuálním buildu nugetu ještě není.</p><pre class=""brush: csharp"">runFor [chrome; firefox; ie]</pre>
<h2>Smoke test reálného webu</h2>
<p>Na začátku jsem sliboval že si napíšeme testy skutečný web, jako pokusný web použijeme <a href=""http://www.lepsimisto.cz/"">Lepší místo</a>. Uděláme si jednoduchou sadu smoke testů, které můžou vývojáři pustit po nasazení, aby ověřil základní funkčnost aplikace. </p>
<p>Jako první si napíšeme test ověřující funkčnost vyhledávání, test je velmi podobný testu vyhledávání na google.</p><pre class=""brush: csharp"">""search Oprava laveček v nádražní budově Kr.Pole"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/""

    ""#searching"" &lt;&lt; ""Oprava laveček v nádražní budově Kr.Pole""
    press enter

    click ""section.search-results article.bottom div.content div.item h3 a""

    on ""http://www.lepsimisto.cz/tip/oprava-lavecek-v-nadrazni-budove-krpole""
</pre>
<p>Na tomhle pro nás ale není nic nového. Pojďme zkontrolovat jestli se na mapě zobrazuje výpis tipů, problémem je že se tipy do načítají až po načtení stránky. Nejjednodušším způsobem jak otestovat, takovou to situaci je sleep.</p><pre class=""brush: csharp"">""maps show Announcements"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/mapa""
    sleep 5
    displayed (element ""#announcement-list .item"")
</pre>
<p>Tohle není ale moc pěkné řešení a hlavně není deterministické. Mnohem lepším řešením je počkat si až se data načtou a pak je otestovat. </p><pre class=""brush: csharp"">""maps show Announcements"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/mapa""
    let itemsIsDisplayed() =  (elements ""#announcement-list .item"").Length &gt; 3
    waitFor itemsIsDisplayed</pre>
<p>Vím že testy nejsou shodné, ale jde mi o ukázku možností Canopy. Metoda waitFor čeká dokud itemsIsDisplayed nevrátí true, pokud to nestihne do timeout (defaultně 10s) test failne. </p>
<p>Dalším testem otestujeme přepínání jazyků</p><pre class=""brush: csharp"">""check translation works"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    click (first ""#language-selector a"")
    contains ""Our independence""  (read "".about-content"")

    click (last ""#language-selector a"")
    contains ""Naše nezávislost"" (read "".about-content"")
</pre>
<p>Ale co když chceme zjistit jetli je daný text na stránce nezávisle na jazyku? Potřebovali by jsme test, kterému předhodíme tři stringy a on nám řekl jestli třetí string obsahuje jeden z předchozích dvou. Jenže takový assert v Canopy není. Naštěstí dopsat si vlastní assert není těžké. Assert je funkce, která nic nevrátí a pokud assert padne vyhodí výjimku. Takže námi požadovaný assert by vypadal nějak takhle </p><pre class=""brush: csharp"">let containsOneOf (value1 : string) (value2 : string) (value3 : string) =
    if (value3.Contains(value1) &lt;&gt; true) &amp;&amp;
       (value3.Contains(value2) &lt;&gt; true)  then
        raise (InvalidOperationException(sprintf ""contains check failed.  %s does not contain %s or %s"" value3 value1 value2))
</pre>
<p>Samotný test</p><pre class=""brush: csharp"">""about project"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    containsOneOf ""Naše nezávislost"" ""Our independence""  (read "".about-content"")</pre>
<p>Výsledný soubor se smoke testama</p><pre class=""brush: csharp"">open canopy
open runner
open System
open OpenQA.Selenium.Chrome
open configuration

chromeDir &lt;- @"".""

let containsOneOf (value1 : string) (value2 : string) (value3 : string) =
    if (value3.Contains(value1) &lt;&gt; true) &amp;&amp;
       (value3.Contains(value2) &lt;&gt; true)  then
        raise (InvalidOperationException(sprintf ""contains check failed.  %s does not contain %s or %s"" value3 value1 value2))

""about project"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    containsOneOf ""Naše nezávislost"" ""Our independence""  (read "".about-content"")

""check translation works"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    click (first ""#language-selector a"")
    contains ""Our independence""  (read "".about-content"")

    click (last ""#language-selector a"")
    contains ""Naše nezávislost"" (read "".about-content"")

""maps show Announcements"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/mapa""
    let itemsIsDisplayed() =  (elements ""#announcement-list .item"").Length &gt; 3
    waitFor itemsIsDisplayed

""search Oprava laveček v nádražní budově Kr.Pole"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/""
    ""#searching"" &lt;&lt; ""Oprava laveček v nádražní budově Kr.Pole""
    press enter

    click ""section.search-results article.bottom div.content div.item h3 a""
    on ""http://www.lepsimisto.cz/tip/oprava-lavecek-v-nadrazni-budove-krpole""

start firefox
run()

start chrome
run()

quit()</pre>
<h2>ZÁZNAM TESTŮ</h2>
<p>Hodně praktickou funkcí je možnost pořídit screenshot. Napíšeme si na to jednoduchou pomocnou funkci</p><pre class=""brush: csharp"">let testPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @""\canopy\"" + DateTime.Now.ToString(""yyyy-MM-dd_HH-mm-ss"")

let takeScreenshot(screenshotName : string) =
    let path = testPath + ""\\"" + browser.ToString()
    screenshot path screenshotName
</pre>
<p>Stačí ji zavolat kdekoli s testu</p><pre class=""brush: csharp"">takeScreenshot ""search""</pre>
<p>Další žádanou vlastností je export výsledků testů do html. Stačí pár snadných úprav</p><pre class=""brush: csharp"">open canopy
open runner
open System
open OpenQA.Selenium.Chrome
open configuration
open reporters

chromeDir &lt;- @"".""

let startTime = DateTime.Now.ToString(""yyyy-MM-dd_HH-mm-ss"")
let path = fun _ -&gt; Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @""\canopy\"" + startTime + ""\\"" + browser.ToString()

let takeScreenshot (screenshotName : string) =
    let pathBrowserFolder = path()
    screenshot pathBrowserFolder screenshotName

let containsOneOf (value1 : string) (value2 : string) (value3 : string) =
    if (value3.Contains(value1) &lt;&gt; true) &amp;&amp;
       (value3.Contains(value2) &lt;&gt; true)  then
        raise (InvalidOperationException(sprintf ""contains check failed.  %s does not contain %s or %s"" value3 value1 value2))

context ""tests""
""about project"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    containsOneOf ""Naše nezávislost"" ""Our independence""  (read "".about-content"")

""check translation works"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/o-projektu""
    click (first ""#language-selector a"")
    contains ""Our independence""  (read "".about-content"")

    click (last ""#language-selector a"")
    contains ""Naše nezávislost"" (read "".about-content"")

""maps show Announcements"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/mapa""
    let itemsIsDisplayed() =  (elements ""#announcement-list .item"").Length &gt; 3
    waitFor itemsIsDisplayed

""search Oprava laveček v nádražní budově Kr.Pole"" &amp;&amp;&amp; fun _ -&gt;
    url ""http://www.lepsimisto.cz/""
    ""#searching"" &lt;&lt; ""Oprava laveček v nádražní budově Kr.Pole""
    press enter

    takeScreenshot ""search""

    click ""section.search-results article.bottom div.content div.item h3 a""
    on ""http://www.lepsimisto.cz/tip/oprava-lavecek-v-nadrazni-budove-krpole""

reporter &lt;- new LiveHtmlReporter() :&gt; IReporter
let chromeReporter = reporter :?&gt; LiveHtmlReporter
start chrome
run()
let chromePath = path()
chromeReporter.saveReportHtml chromePath ""report""

reporter &lt;- new LiveHtmlReporter() :&gt; IReporter
let firefoxReporter = reporter :?&gt; LiveHtmlReporter
start firefox
run()
let firefoxPath = path()
firefoxReporter.saveReportHtml firefoxPath ""report""

quit()</pre>
<p>Výsledek testů najdete <strong>%appdata%\canopy</strong></p>
<p><a href=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_17.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/CanopyUI-testovn_6773/image_thumb_7.png"" width=""640"" height=""150""></a></p>
<p>Další možností je TeamCity report, o tom ale až příště.</p>
<p>A pokud se večer nemáte na co dívat přidejte k <strong>&amp;&amp;&amp;</strong> ještě jeden <strong>&amp;&amp;&amp;&amp;</strong>, testy se pustí v pomalém módu a zvýrazňují co zrovna dělají.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8457, 3, CAST(N'2015-05-17T10:43:07.737' AS DateTime), N'Migrace aplikace na .NET Core', N'<p>Včera jsem se rozhodl v praxi vyzkoušet, jak snadné nebo složité je zmigrovat kód z .NETu 4.5 na .NET Core – vzal jsem <a href=""http://github.com/riganti/redwood"">Redwood</a> a vytvořil jsem větev dotnetcore, která obsahuje (zatím ještě nezkompilovatelnou) verzi.</p> <p>Redwood jsem psal s ohledem na to, že tu migraci budu v budoucnu provádět a tím pádem jsem se snažil vyvarovat použití věcí, o kterých jsem věděl, že v novém .NET Core nebudou (zejména cokoliv ze System.Web atd.).</p> <p>I tak jsem ale narazil na pár věcí, které bude potřeba upravit či řešit jinak – stále mi tam visí 19 kompilačních chyb a to jsem řešil zatím jen projekt Framework, neřešil jsem jeho použití v ASP.NET aplikaci (i když to už nebude tak složité).</p> <p>Každopádně pokud chcete migraci provádět, dejte si pozor na následující (tento seznam rozhodně není úplný – je v něm jen to, na co jsem při migraci Redwoodu narazil): </p> <p>&nbsp;</p> <h3>Runtime T4 šablony</h3> <p>T4 šablony spouštěné za běhu (více v článku <a href=""http://www.dotnetportal.cz/clanek/158/Pouzivame-T4-sablony"">Používáme T4 šablony</a>). Tyto šablony fungují tak, že z nich Visual Studio vygeneruje třídu, která obsahuje vše, co nadeklarujete v šabloně uvnitř bloků ohraničených &lt;#+&nbsp; #&gt;, a funkci TransformText, která celou transformaci spustí.<br>Bohužel to, co VS standardně generuje, má závislosti na namespace System.CodeDom, který v .NET Core není a nebude (nahradil ho Roslyn).</p> <p>Navíc Visual Studio ani v projektu pro nový framework T4 šablony nepodporuje a nevygeneruje z nich patřičný výstup. Vzhledem k tomu, že jsem šablony potřeboval, a vzhledem k tomu, že pro automatizaci procesů při vývoji se dá využít <a href=""http://www.dotnetportal.cz/clanek/8456/Jak-se-vyporadat-s-LESS-soubory-ve-Visual-Studiu-2015"">Grunt</a>, napsal jsem do něj (v javascriptu – grr!!) vlastní task <a href=""https://github.com/riganti/grunt-runtime-t4-template-task"">grunt-runtime-t4-template-task</a>, který poštvete na T4 šablonu a ona z ní vygeneruje patřičnou C# třídu. Tato třída nemá závislosti na CodeDomu, jediné omezení je, že nepodporuje metody jako <em>PushIndent</em>, <em>PopIndent</em> a pár dalších věcí – nicméně ten základ, že vytvoříte instanci šablony, naplníte ji daty a zavoláte <em>TransformText</em>, ten pochopitelně funguje. </p> <p>Vyzkoušel jsem si při tom <a href=""https://code.visualstudio.com/"">Visual Studio Code</a> a na jednodušší věci to není úplně špatné, dokonce se mi povedlo donutit ho, aby mi ten grunt task spustil a uměl v něm debugovat, což mě velmi potěšilo.</p> <p>&nbsp;</p> <h3>RESX soubory</h3> <p>Další podobnou libůstkou, kterou ještě vyřešenou nemám, je kompilace RESX souborů. Člověk byl zvyklý, že v projektu vytvořil soubor MyResources.resx, což na pozadí vygenerovalo třídu MyResources, která měla jednotlivé položky zpřístupněné jako readonly properties, takže stačilo napsat MyResources.MyErrorMessage a vrátilo to danou položku v aktuálním jazyce.</p> <p>Nic takového v .NET Core opět není, takže jsem napsal ještě grunt task <a href=""https://github.com/riganti/grunt-resx-compile-task"">grunt-resx-compile-task</a>, který tohle také řeší – vygeneruje třídu, která je zkompilovatelná v .NET Core.</p> <p>&nbsp;</p> <h3>NuGet téměř balíčky pro každý namespace</h3> <p>.NET Core dramaticky změnil rozdělení jednotlivých tříd do assemblies. Vše je velmi modulární, takže třeba System.IO, System.Reflection, System.Collections apod. jsou separátní knihovny, které se distribuují pomocí NuGetu. </p> <p>Má to svůj smysl, protože framework není jeden monolitický balík, který se updatuje jednou za 2 roky, nicméně aby to bylo použitelné, bylo by krásné, kdyby člověk ve chvíli, kdy ve Visual Studiu napíše název nějaké třídy, aby mu to zobrazilo nejen namespace, ale i ze kterého NuGet balíčku pochází a hlavně aby to snadno umožnilo dotáhnout balíčky, které chybí (o což se VS do jisté míry snaží, ale zatím to moc nefunguje).</p> <p>V souboru package.json pak bohužel IntelliSense nenabízí všechny balíčky, které by měla, takže člověk píše a hádá naslepo, jak by se daný balíček mohl jmenovat.</p> <p>Velmi užitečná služba je <a title=""http://packagesearch.azurewebsites.net/"" href=""http://packagesearch.azurewebsites.net/"">http://packagesearch.azurewebsites.net/</a>, kde zadáte název třídy nebo metody a ono vám to najde balíčky, které tuto metodu obsahují. Je také dobré se podívat na složku <a title=""https://github.com/dotnet/corefx/tree/master/src"" href=""https://github.com/dotnet/corefx/tree/master/src"">https://github.com/dotnet/corefx/tree/master/src</a>, kde je velmi dobře vidět, na jaké balíčky byl .NET rozdělen – v zásadě platí, že co složka, to NuGet balíček.<sub></sub></p> <p>&nbsp;</p> <h3>AppDomain a Assembly</h3> <p>Vzhledem k tomu, že Redwood je samá reflection, narazil jsem na mnoho kompilačních chyb. Autoři .NET Core totiž udělali jednu drobnou změnu, a to zeštíhlení třídy System.Type. Spousta vlastností a metod, které tam byly, jsou nyní dostupné ne přímo ze System.Type, ale musíte na ní zavolat extension metodu GetTypeInfo, jejíž výsledek teprve obsahuje to, co potřebujete (např. vlastnosti Assembly, IsValueType atd.). Takže jsem to musel asi na 150 místech měnit. To byl ovšem ten menší problém.</p> <p>Zbylo mi tam 15 kompilačních chyb, se kterými se budu muset vypořádat, jelikož .NET Core změnil některé základní principy. Třída Assembly má pouze metodu Load, která umí načíst assembly dle názvu. Není tam žádná funkce, která by uměla načíst assembly ze streamu nebo z pole bajtů – pokud ano, tak se jmenuje jinak a nepovedlo se mi ji zatím najít. Oblíbené a často používané funkce <strong>Assembly.GetExecutingAssembly</strong> chybí.</p> <p>Podobně je na tom třída AppDomain – pokud jste ve starém .NETu chtěli zjistit seznam assemblies, které jsou ve vaší appdoméně načtené, zavolali jste <strong>AppDomain.CurrentDomain.GetAssemblies</strong>. Tato funkce zde není, takže budu muset najít jiný způsob, jak se s tím vypořádat.</p> <p>&nbsp;</p> <h3>OWIN</h3> <p>Nedávno Microsoft začal humbuk kolem OWINu, a tak jsem Redwood naimplementoval na Owinu, nicméně teď jsem jej musel upravit tak, aby nepoužíval OWIN, ale nový ASP.NET runtime. Prakticky je rozdíl jen v pojmenování tříd (IApplicationBuilder místo IAppBuidler, RequestDelegate místo IOwinMiddleware apod.) a vše je v jiných namespacech, principy naštěstí zůstaly stejné, takže se jednalo jen o relativně snadné úpravy. </p> <p>&nbsp;</p> <p>Každopádně udržovat dvě verze frameworku pro nový a starý .NET možná nebude tak jednoduché – budu muset zjistit, jestli je to reálně možné. Projekt ve VS to sice podporuje, nicméně vzhledem k tomu, že framework používá věci z nového ASP.NET, nebude to zřejmě tak snadné udělat, abych měl jeden kód pro obě platformy. Uvidíme.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8459, 15, CAST(N'2015-08-17T22:05:52.260' AS DateTime), N'Tracing v .NET pomoci Event Tracing for Windows (ETW)', N'<p><em>Předem bych nejprve chtěl uvést, že rozhodně nejsem žádný expert na ETW technologii (z článku to asi bude vidět), pouze zde popíši informace, které jsem získal, když jsem technologii chtěl použít v mé aplikaci. Také mé překvapilo, že jsem nikde nenašel právě takovýto popis pro programátory co s technologii začínají a chtějí v co nejkratším čase implementovat základní trace s využitím ETW do aplikace. Pokud tedy máte s technologii nějaké zkušenosti, rozhodně se o ně podělte v komentářích.</em></p> <p><em><font color=""#0000ff"">Rozšíření článku modrou barvou.</font></em></p> <h3>Event Tracing for Windows (ETW)</h3> <p>Technologie <strong>Event Tracing for Windows (ETW)</strong> je tady s námi od systému <strong>Windows Vista</strong>. Jedná se o technologii pro sběr a zápis trasovacích informací z aplikací. Hlavní důvod proč použít tuto technologii je to, že díky její implementaci přímo v jádře se jedná o extrémně výkonné řešení pro zápis trasovacích informací. Díky tomu není aplikace při používání trace skoro vůbec výkonnostně ovlivněná.</p> <p>Technologie má širokou podporu napříč Windows platformou, je rozšiřitelná, připravená i na distribuované prostředí, technologii dnes využívá již velký počet aplikací a služeb Windows jako např. IIS, SQL Server a .NET samotný. Trasovací informace můžeme přímo zaznamenávat a analyzovat prostředky systému, nebo pro ETW existují i různé nástroje třetích stran. Dále pro řešení na platformě Microsoft Azure je možné záznamy ukládat do sdíleného úložiště pomocí technologie <em>Windows Azure Diagnostic</em>.</p> <h3>ETW Providers</h3> <p>Zápis událostí probíhá pomoci ETW <strong>Providers</strong>. Tyto poskytovatelé poskytuje daná aplikace a mohou být zaregistrované ve Windows.</p> <p>Pro jejich výpis slouží příkaz:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">xperf -providers I</pre>
<p>nebo</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">logman query providers</pre>
<p>Vidíme, že ve Windows je těchto providerů již docela hodně, samotný .NET Framework jich používá několik, zkuste si příkaz:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">xperf -providers I | Findstr "".NET""

a669021c-c450-4609-a035-5af59af4df18                              : Microsoft-Windows-DotNETRuntimeRundown
aff081fe-0247-4275-9c4e-021f3dc1da35                              : ASP.NET Events
bd568f20-fccd-b948-054e-db3421115d61                              : DBNETLIB.1
e13c0d23-ccbc-4e12-931b-d9cc2eee27e4                              : .NET Common Language Runtime
e13c0d23-ccbc-4e12-931b-d9cc2eee27e4                              : Microsoft-Windows-DotNETRuntime
</pre>
<p>Všimněte si, že každý provider má svůj jedinečný <strong>GUID</strong>, např. pro .NET CLR slouží provider <strong>.NET Common Language Runtime</strong> s GUID: e13c0d23-ccbc-4e12-931b-d9cc2eee27e4.</p>
<h3>Zaznamenání událostí</h3>
<p>Zkusme si zaznamenat události z tohoto <em>.NET Common Language Runtime</em> provideru. Výhodou je, že zaznamenání můžeme zapnout klidně až poté co aplikace běží, nemusíme jí registrovat nebo restartovat. Pro zaznamenání použijeme jeden z příkazů:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">xperf -start clr -on "".NET Common Language Runtime"" -f clrevents.etl
xperf -start clr -on e13c0d23-ccbc-4e12-931b-d9cc2eee27e4 -f clrevents.etl</pre>
<p>kde spouštíme zaznamenávání pod jménem <em>clr</em> z providera určeného buď jménem (první varianta) nebo GUIDem, do souboru <strong>clrevents.etl</strong>.</p>
<p>Zastavení provedeme příkazem:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">xperf -stop clr</pre>
<p>Pokud uložený soubor <em>clrevents.etl</em> otevřeme přímo, najede nám nástroj <strong>Windows Performance Analyzer</strong>, ve kterém sice vidíme časové rozložení událostí, ale události jsou bez veškerých popisů. Jak jsem pochopil tak by se muselo pomoci Windows Performance Toolkit vytvořit nějaké popisné xml (soubor .wprp).</p>
<p>Pro naší potřebu zobrazit zaznamenané události použijeme jiný nástroj - <strong>PerfView.exe</strong> – ke stažení je <a href=""http://www.microsoft.com/en-us/download/details.aspx?id=28567"" target=""_blank"">zde</a>.</p>
<p>Po spuštění <strong>PerfView</strong> vybereme složku se souborem a v něm zvolíme prvek <strong>Events</strong>.</p>
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/1d71e3bd38f2_9A2D/image_3.png"" width=""811"" height=""547""></p>
<p>V Events okně pak již vidíme zaznamenané události, na obrázku jsou např. zobrazeny výskyty jedné událostí .NET Garbage Collectoru. </p>
<p><font color=""#0000ff"">Ještě je jiná možnost spuštění zaznamenání událostí přímo pomoci nástroje PrefView příkazem:<br></font><font color=""#0000ff""><em>PerfView /onlyProviders=e13c0d23-ccbc-4e12-931b-d9cc2eee27e4 collect<br>PerfView /onlyProviders="".NET Common Language Runtime"" collect</em></font></p>
<p>Tak to by jako krátký úvod stačilo, nyní se již podíváme jak ETW naimplementovat do naší .NET aplikace.</p>
<h3>Implementace v .NET</h3>
<p>V prostředí .NET je podpora pro technologii ETW až od verze .NET 4.5 pomoci třídy <em><a href=""https://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&amp;l=EN-US&amp;k=k(System.Diagnostics.Tracing.EventSource);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5.1);k(DevLang-csharp)&amp;rd=true"" target=""_blank"">System.Diagnostics.Tracing.EventSource</a></em>. Druhou možnosti je použít <strong>Nuget balíček </strong><a href=""https://www.nuget.org/packages/Microsoft.Diagnostics.Tracing.EventSource"" target=""_blank""><strong>Microsoft.Diagnostics.Tracing.EventSource</strong></a>, který nám jednak umožňuje technologii ETW využít i ve starších .NET Frameworkcích (ale pozor, že ve Windows XP technologie stejně dostupná není, takže není moc důvod nepřejít na .NET 4.5). Druhou volbou, proč po Nugetu šáhnout může být to, že v něm jsou některé rozšířené funkcionality, které se zatím do .NET Frameworku nedostali. Já zatím zůstanu u té standardní implementace součásti Frameworku. 
<p>Vytvořením vlastní třídy odvozené z třídy <strong>EventSource</strong> implementujeme náš vlastní ETW provider. V něm deklarujeme, které události budeme chtít v aplikaci tracovat. Ukážeme si to rovnou na příkladu takové třídy, kterou jsem v aplikaci použil:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">/// &lt;summary&gt;
/// Event Tracing for Windows (ETW) event provider for application
/// &lt;/summary&gt;
/// &lt;remarks&gt;
/// Capture events command:
/// xperf -start usersession -on 71C65474-E573-4117-B374-A8BC8205860A -f usersession.etl
/// xperf -stop usersession
/// &lt;/remarks&gt;
[EventSource(Name = ""MyApplication-Tracing-Provider"", Guid = ""{71C65474-E573-4117-B374-A8BC8205860A}"")]
public sealed class MyTraceEventSource : EventSource
{
    public static MyTraceEventSource Trace = new MyTraceEventSource();

    private MyTraceEventSource() { }

    #region action methods
    //if the order of the methods don’t match ordinal number position in the class it would fail generating ETW traces.
    //The EventSource has dependency on the order of the methods in the class.

    [Event(1, Level = EventLevel.Informational)]
    public void ApplicationInfo(string message) { WriteTrace(1, message); }

    [Event(2, Level = EventLevel.Warning)]
    public void ApplicationWarning(string message) { WriteTrace(2, message); }

    [Event(3, Level = EventLevel.Error)]
    public void Exception(string message) { WriteTrace(3, message); }

    [Event(4, Level = EventLevel.Informational)]
    public void SQLTrace(string message) { WriteTrace(4, message); }
    #endregion

    #region private member functions
    private void WriteTrace(int id, string message)
    {
#if DEBUG
        string eventName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
        var eventAttribute = (EventAttribute)new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().GetCustomAttributes(typeof(EventAttribute), false)[0];

        System.Diagnostics.Debug.WriteLine($""{eventName} ({eventAttribute.Level.ToString()}): {message}"");
#endif
        if (IsEnabled())
        {
            WriteEvent(id, message);
        }
    }
    #endregion
}</pre>
<p>Při implementaci této třídy odvozené z <strong>EventSource</strong> to ale není tak jednoduché. Sice se po nás požaduje pouze volání metody <strong>WriteEvent</strong>. Já to zde tak činím ještě pomoci metody <em>WriteTrace</em>, ve které zapisuji i do standardního debug výstupu, aby se mi události vypisovali i ve Visual Studiu při laděni (protože Visual Studio nemá žádnou automatickou podporu na vypisování ETW událostí). 
<p>V čem je tedy problém s <strong>EventSource</strong> třídou? Všimněte si, že se nikde neuvádí <strong>jméno trace události</strong>, jméno události je totiž odvozeno samo pomoci reflekce z této třídy. Přičemž musí platit, že <strong>event id</strong>, se kterým se <em>WriteEvent</em> volá, musí být stejné jako to, které se uvede v <strong><em>Event</em></strong> atributu (pokud atribut použijeme, což doporučuji). Dále také záleží na pořadí metod ve třídě - ty musí být uvedeny ve stejném pořadí jako jsou použité event id v jejich volání. Z toho také plyne, že každý event musíme zavést natvrdo jako metodu, nemůžeme například mít jednu obecnou metodu, ve které by jsme event id posílali parametrem. 
<p>Použití třídy pro volání zápisu trace události pak bude v naší aplikaci například takto:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">MyTraceEventSource.Trace.ApplicationInfo(""Initializing..."");</pre><font color=""#0000ff"">Při použití verze z NuGet balíčku <strong>Microsoft.Diagnostics.Tracing.EventSource.Redist</strong> nebo použití .NET Framework 4.6 je třída <strong>EventSource</strong> rozšířená o možnost tvz. <strong>DynamicEvents</strong>, tj lze definovat události přímo voláním v kódu. Výkonnost je ovšem o malinko nižší než v případě těch contract based událostí. Zápis dynamické události se provádí takto:<br><em>MyTraceEventSource.Trace.Write(""DynamicEvent"", new { id = 5, arg = message });</em></font> 
<p>Po spuštění aplikace necháme události zaznamenat dříve popsaným způsobem, např. příkazy:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">xperf -start usersession -on 71C65474-E573-4117-B374-A8BC8205860A -f usersession.etl

xperf -stop usersession</pre>
<p>Zde volám providera určeného pomoci GUID, které je uvedeno v atributu u naší EventSource třídy, což funguje. <strike>Nepřišel jsem ale na to, jak našeho providera zaregistrovat do systému (aby byl v providerech seznamu xperf -providers I) a jak mu přiřadit nějaké jméno, aby šlo zaznamenávání volat kromě GUIDu i s určením tohoto jména</strike>.
<p><font color=""#0000ff"">Abychom nepotřebovali GUID, tak můžeme zaznamenávání spustit pomoci<br><em>PerfView /onlyProviders=*MyApplication-Tracing-Provider collect</em></font>
<p><font color=""#0000ff"">A na to jak event zaregistrovat do systému jsem také už našel odpověď, je to spojeno s funkcí <strong>ETW Channel</strong>. Nejprve musíme nechat při kompilaci vytvořit soubory <strong>.man</strong> a <strong>manifest .dll</strong>. To provedeme tak, že to projektu nainstalujeme NuGet balíček <a href=""https://www.nuget.org/packages/Microsoft.Diagnostics.Tracing.EventRegister/"" target=""_blank""><strong>Microsoft.Diagnostics.Tracing.EventRegister</strong></a>. Tento balíček vloží do MSBuild procesu volání utility <em>eventRegister.exe</em>, která po kompilaci ve výstupním adresáři vytvoří soubory:<br></font><em><strong>&lt;AssemblyName&gt;.&lt;EventSourceTypeName&gt;.etwManifest.man<br>&lt;AssemblyName&gt;.&lt;EventSourceTypeName&gt;.etwManifest.dll</strong></em><br><font color=""#0000ff"">K tomu je ještě nutné, aby byla v EventSource třídě alespoň jedna událost v atributu <strong>Event</strong> s parametrem <strong>Channel</strong> (a při EventChannel.Admin ještě parametr Message). Také se doporučuje při tomto postupu GUID ve třídě neuvádět.</font></p>
<p><font color=""#0000ff"">Nyní musíme tyto soubory zaregistrovat příkazem:<br><em>wevtutil.exe im ""&lt;EtwManifestManFile&gt;"" /rf:""&lt;EtwManifestDllFullPathName&gt;"" /mf:""&lt;EtwManifestDllFullPathName&gt;""<br>neboli<br>wevtutil.exe im ""c:\..\ETWTest.MyApplication-Tracing-Provider.etwManifest.man"" /rf:""c:\..\ETWTest.MyApplication-Tracing-Provider.etwManifest.dll"" /mf:""c:\..\ETWTest.MyApplication-Tracing-Provider.etwManifest.dll""</em><br>Nyní <em>xperf -providers I | Findstr ""MyApplication""</em> bude vracet zaregistrovaný provider v systému.</font></p>
<p><font color=""#0000ff"">Pokud použijeme <strong>Channel Admin</strong> nebo <strong>Operational</strong>, tak se zároveň budou události <strong>zapisovat i do EventLogu</strong>, který se registrací vytvoří v sekci <strong><em>Application and Services Logs</em></strong> podle názvu našeho EventSource:</font></p>
<p><img title=""image"" style=""border-top: 0px; border-right: 0px; background-image: none; border-bottom: 0px; padding-top: 0px; padding-left: 0px; border-left: 0px; display: inline; padding-right: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/1d71e3bd38f2_9A2D/image_7.png"" width=""240"" height=""291""></p>
<p><font color=""#0000ff"">K odregistraci pak slouží příkaz:<br><em>wevtutil.exe um &lt;EtwManifestManFile&gt;</em></font></p>
<p><font color=""#0000ff"">Tak a ještě jedna možnost jak zaznamenávat události, přímo pomoci <strong>Performance Monitoru</strong>. V sekci <em>Data Collector Sets\Event Trace Sessions</em> můžeme v definici nového Data Collection Set vybrat náš zaregistrovaný EventSource.</font></p>
<p><img title=""image"" style=""border-top: 0px; border-right: 0px; background-image: none; border-bottom: 0px; padding-top: 0px; padding-left: 0px; border-left: 0px; display: inline; padding-right: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/1d71e3bd38f2_9A2D/image_10.png"" width=""664"" height=""365""></p>
<p><font color=""#0000ff""><br></font>Po zaznamenání a zobrazení v PerfView opravdu vidíme v seznamu naše události definované jednotlivými metodami naší třídy. </p>
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/1d71e3bd38f2_9A2D/image_6.png"" width=""692"" height=""358""> 
<h3>Další zroje</h3>
<p>Některé další zdroje, ze kterých jsem čerpal: 
<p><a title=""http://geekswithblogs.net/akraus1/archive/2013/08/21/153804.aspx"" href=""http://geekswithblogs.net/akraus1/archive/2013/08/21/153804.aspx"" target=""_blank"">http://geekswithblogs.net/akraus1/archive/2013/08/21/153804.aspx</a><br><a title=""http://blogs.msdn.com/b/vancem/archive/2012/07/09/logging-your-own-etw-events-in-c-system-diagnostics-tracing-eventsource.aspx"" href=""http://blogs.msdn.com/b/vancem/archive/2012/07/09/logging-your-own-etw-events-in-c-system-diagnostics-tracing-eventsource.aspx"" target=""_blank"">http://blogs.msdn.com/b/vancem/archive/2012/07/09/logging-your-own-etw-events-in-c-system-diagnostics-tracing-eventsource.aspx</a><br><a title=""http://naveensrinivasan.com/category/net/etw-net/"" href=""http://naveensrinivasan.com/category/net/etw-net/"" target=""_blank"">http://naveensrinivasan.com/category/net/etw-net/</a><br><a title=""EventSource Users Guide"" href=""http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-components-postattachments/00-10-44-08-22/_5F00_EventSourceUsersGuide.docx"" target=""_blank"">http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-components-postattachments/00-10-44-08-22/_5F00_EventSourceUsersGuide.docx</a><br><a title=""https://github.com/mspnp/azure-guidance/blob/master/Monitoring.md"" href=""https://github.com/mspnp/azure-guidance/blob/master/Monitoring.md"" target=""_blank"">https://github.com/mspnp/azure-guidance/blob/master/Monitoring.md</a><br><font color=""#0000ff"">Velkým zdrojem jsou příklady a dokumentace obsažené v NuGet balíčku <strong><a href=""https://www.nuget.org/packages/Microsoft.Diagnostics.Tracing.EventSource.Samples/"" target=""_blank"">Microsoft EventSource Library Samples.</a></strong></font></p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8460, 15, CAST(N'2015-07-13T17:50:16.413' AS DateTime), N'Asynchronní volání pro Entity Framework a AsEnumerable', N'<p><em>Entity framework sám o sobě již nějakou dobu podporuje asynchronní volání. Při načítání dat pomoci entity frameworku je ale dost často využívaná metoda AsEnumerable, pomoci které můžeme části dotazu, které nelze provést přímo v SQL, nechat v druhém kroku vyhodnotit až na klientu. Takovýto dotaz ale standardně asynchronně volat nelze. Jak na to, si ukážeme v tomto článku.</em></p> <h3>Synchronní verze</h3> <p>Mějme následující dvě metody využívající pro načítání dat Entity Framework v kombinaci s použitím metody <em><a href=""https://msdn.microsoft.com/en-us/library/vstudio/bb335435(v=vs.110).aspx"" target=""_blank"">AsEnumerable</a></em>:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">public static class UzivateleService
{
    #region action methods
    public static IList&lt;Models.Uzivatel&gt; GetUzivatele(bool zahrnoutVyrazene)
    {
        using (var context = new DataContext())
        {
            var uzivatele = GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziPlatnostDo == null || zahrnoutVyrazene)).ToList();
            return uzivatele;
        }
    }

    public static Models.Uzivatel GetUzivatel(int IDUzivatele)
    {
        using (var context = new DataContext())
        {
            return GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziIDUzivatele == IDUzivatele)).FirstOrDefault();
        }
    }
    #endregion

    #region private member functions
    private static IEnumerable&lt;Models.Uzivatel&gt; GetUzivatele(IQueryable&lt;Uzivatel&gt; source)
    {
        var uzivatele = from result in
                            (from u in source
                            orderby u.uziPrijmeni, u.uziJmeno
                            select new
                            {
                                IDUzivatele = u.uziIDUzivatele,
                                Zapsano = u.uziZapsano,
                                Jmeno = u.uziJmeno,
                                Prijmeni = u.uziPrijmeni,
                                UzivatelLogin = u.UzivatelLogin,
                                Prihlaseni = (from prihlaseni in u.UzivatelPrihlaseni
                                                orderby prihlaseni.up_PosledniPrihlaseni descending
                                                select prihlaseni).FirstOrDefault(),
                                Vyrazen = u.uziPlatnostDo != null
                            }).AsEnumerable()
                        select new Models.Uzivatel()
                        {
                            IDUzivatele = result.IDUzivatele,
                            Zapsano = result.Zapsano,
                            Jmeno = result.Jmeno,
                            Prijmeni = result.Prijmeni,
                            PrihlasovaciJmena = string.Join("", "", result.UzivatelLogin.Select(o =&gt; o.ul_LoginName)),
                            PosledniPrihlaseni = result.Prihlaseni == null ? (DateTime?)null : result.Prihlaseni.up_PosledniPrihlaseni,
                            PosledniPrihlaseniPocitac = result.Prihlaseni == null ? null : result.Prihlaseni.up_PosledniPrihlaseniPocitac,
                            PosledniPrihlaseniOS = result.Prihlaseni == null ? null : result.Prihlaseni.up_PosledniPrihlaseniOS,
                            Vyrazen = result.Vyrazen
                        };

        return uzivatele;
    }
    #endregion
}</pre>
<p>Kód je vcelku jednoduchý a jasný, obě metody <em>GetUzivatele</em> i <em>GetUzivatel</em> volají pomocnou metodu, která obsahuje logiku pro transformaci entity na model. Třídu entity ani modelu zde neuvádím, protože to jak vypadají je z příkladu zřejmé. Pomocná privátní metoda <em>GetUzivatele</em> používá v EF LINQ dotazu metodu <em>AsEnumerable</em> z toho důvodu, aby volání <em>string.Join()</em> proběhlo až na klientu po načtení dat z SQL Serveru (protože toto volání nelze přeložit a provést přímo součástí SQL dotazu).</p>
<h3>Problém</h3>
<p>Pokud tento kód budeme chtít převést na asynchronní verzi, narazíme. Pro asynchronní volání se v EF používají <em><a href=""https://msdn.microsoft.com/en-us/library/system.data.entity.queryableextensions(v=vs.113).aspx"" target=""_blank"">QueryableExtensions</a></em> z namespace <em>System.Data.Entity</em>. Ty poskytují extension metody jako <em><a href=""https://msdn.microsoft.com/en-US/library/system.data.entity.queryableextensions.tolistasync(v=vs.113).aspx"" target=""_blank"">ToListAsync</a></em>, <em><a href=""https://msdn.microsoft.com/en-US/library/system.data.entity.queryableextensions.firstasync(v=vs.113).aspx"" target=""_blank"">FirstAsync</a></em>, <em><a href=""https://msdn.microsoft.com/en-US/library/system.data.entity.queryableextensions.firstordefaultasync(v=vs.113).aspx"" target=""_blank"">FirstOrDefaultAsync</a></em> a další, které ve standardním případě pouze použijeme místo původních synchronních verzí a je hotovo. V našem případě by jsme tedy konkrétně chtěli použít v metodě <em>GetUzivatele</em> volání metody <em>ToListAsync</em> a v metodě <em>GetUzivatel</em> volání metody <em>FirstOrDefaultAsync</em>.</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">public static async Task&lt;IList&lt;Models.Uzivatel&gt;&gt; GetUzivatele(bool zahrnoutVyrazene, CancellationToken cancellationToken)
{
    using (var context = new DataContext())
    {
        var uzivatele = await GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziPlatnostDo == null || zahrnoutVyrazene)).ToListAsync(cancellationToken);
        return uzivatele;
    }
}

public static async Task&lt;Models.Uzivatel&gt; GetUzivatel(int IDUzivatele, CancellationToken cancellationToken)
{
    using (var context = new DataContext())
    {
        return await GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziIDUzivatele == IDUzivatele)).FirstOrDefaultAsync(cancellationToken);
    }
}</pre>
<p>Tyto asynchronní metody jsou dostupné na rozhraní <em>IQueryable&lt;TSource&gt;</em>, kterým je zpravidla dotaz v EF reprezentován. V našem případě je ale po volaní <em>AsEnumerable</em> objekt reprezentující náš dotaz již typu <em>IEnumerable&lt;T&gt;,</em> kde tyto metody samozřejmě nejsou (a ani principiálně být nemohou, protože operace nad IEnumerable již probíhají v paměti dlouho potom, co vygenerovaný SQL dotaz již proběhl).</p>
<h3>Zavedení IDbAsyncEnumerableExtensions</h3>
<p>Co tedy s tím? V zásadě co tady potřebujeme je reprezentovat náš dotaz nějakým novým objektem jiného typu, na kterém ale bude také dostupná metoda <em>ToListAsync</em> (pro první případ) a <em>FirstOrDefaultAsync</em> (pro druhý případ). Tento objekt by měl jít získat voláním nové metody <em>As&lt;něco&gt;</em> na původním queryable, která se bude používat místo původní metody <em>AsEnumerable</em> u synchronní verze (tj. touto metodou řekneme, že chceme dále v LINQ dotazu pracovat již jen s “asynchronním” enumerable). Dále na nový objekt musíme umět aplikovat projekci (tj. bude muset implementovat metodu <em>Select</em>), přičemž objekt bude muset “vědět”, že při spuštění dotazu některou z asynchronních metod, má nejprve vykonat původní SQL dotaz a pak při enumeraci výsledků (v našem případě v anonymním typu) navíc již v paměti provádět definovanou projekci. Co tedy musíme udělat je přesně všechno toto.</p>
<p>Typ, kterým budeme reprezentovat asynchronní dotaz s možnou projekcí na klientu, bude existující typ z EF <em><a href=""https://msdn.microsoft.com/en-us/library/dn159829(v=vs.113).aspx"" target=""_blank"">IDbAsyncEnumerable&lt;TSource&gt;</a> z</em> namespace <em>System.Data.Entity.Infrastructure</em>. Tento typ je totiž v EF interně použit právě pro asynchronní spouštění queryable dotazů a umožňuje projít výsledky dotazu pomoci “asynchronní verze” enumerátor kontraktu.</p>
<p>Toto si lze demonstrovat například kódem (kde <em>query</em> je libovolný EF dotaz typu <em>IQueryable&lt;T&gt;</em>):</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">var query = /*EF LINQ query*/

var source = (IDbAsyncEnumerable&lt;T&gt;)query;

using (var enumerator = source.GetAsyncEnumerator())
{
    while (true)
    {
        if (!await enumerator.MoveNextAsync(cancellationToken))
        {
            break;
        }

        //Use enumerator.Current
    }
}</pre>
<p>K tomuto typu <em>IDbAsyncEnumerable&lt;TSource&gt;</em> naimplementujeme, jako vlastní extension metody, metody <em>Select</em>, <em>ToListAsync</em>, <em>FirstAsync</em>, <em>FirstOrDefaultAsync</em> a dále k typu <em>IQueryable&lt;T&gt;</em> naimplementujeme extension metodu <em>AsDbAsyncEnumerable&lt;T&gt;</em>.</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">internal static class DbAsyncEnumerableExtensions
{
    #region member types declarations
    private sealed class SelectDbAsyncEnumerable&lt;TSource, TTarget&gt; : IDbAsyncEnumerable&lt;TTarget&gt;
    {
        #region member types declarations
        private struct SelectDbAsyncEnumerator : IDbAsyncEnumerator&lt;TTarget&gt;
        {
            #region member varible and default property initialization
            private readonly IDbAsyncEnumerator&lt;TSource&gt; Source;
            private readonly Func&lt;TSource, TTarget&gt; Selector;
            #endregion

            #region constructors and destructors
            public SelectDbAsyncEnumerator(IDbAsyncEnumerator&lt;TSource&gt; source, Func&lt;TSource, TTarget&gt; selector)
            {
                this.Source = source;
                this.Selector = selector;
            }
            #endregion

            #region action methods
            public Task&lt;bool&gt; MoveNextAsync(CancellationToken cancellationToken)
            {
                return this.Source.MoveNextAsync(cancellationToken);
            }

            public void Dispose()
            {
                this.Source.Dispose();
            }
            #endregion

            #region property getters/setters
            public TTarget Current
            {
                get { return this.Selector(this.Source.Current); }
            }

            object IDbAsyncEnumerator.Current
            {
                get { return this.Current; }
            }
            #endregion
        }
        #endregion

        #region member varible and default property initialization
        private readonly IDbAsyncEnumerable&lt;TSource&gt; Source;
        private readonly Func&lt;TSource, TTarget&gt; Selector;
        #endregion

        #region constructors and destructors
        public SelectDbAsyncEnumerable(IDbAsyncEnumerable&lt;TSource&gt; source, Func&lt;TSource, TTarget&gt; selector)
        {
            this.Source = source;
            this.Selector = selector;
        }
        #endregion

        #region action methods
        public IDbAsyncEnumerator&lt;TTarget&gt; GetAsyncEnumerator()
        {
            return new SelectDbAsyncEnumerator(this.Source.GetAsyncEnumerator(), this.Selector);
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }
        #endregion
    }

    private struct QueryableWrapper&lt;TSource&gt; : IQueryable&lt;TSource&gt;, IDbAsyncEnumerable&lt;TSource&gt;
    {
        #region member varible and default property initialization
        private readonly IDbAsyncEnumerable&lt;TSource&gt; Source;
        #endregion

        #region constructors and destructors
        public QueryableWrapper(IDbAsyncEnumerable&lt;TSource&gt; source)
        {
            this.Source = source;
        }
        #endregion

        #region action methods
        public IDbAsyncEnumerator&lt;TSource&gt; GetAsyncEnumerator()
        {
            return this.Source.GetAsyncEnumerator();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }

        public IEnumerator&lt;TSource&gt; GetEnumerator()
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region property getters/setters
        public Type ElementType
        {
            get { return typeof(TSource); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { throw new NotSupportedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotSupportedException(); }
        }
        #endregion
    }
    #endregion

    #region action methods
    public static IDbAsyncEnumerable&lt;TTarget&gt; Select&lt;TSource, TTarget&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source, Func&lt;TSource, TTarget&gt; selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return new SelectDbAsyncEnumerable&lt;TSource, TTarget&gt;(source, selector);
    }

    public static Task&lt;List&lt;TSource&gt;&gt; ToListAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return GetQueryableWrapper(source).ToListAsync();
    }

    public static Task&lt;List&lt;TSource&gt;&gt; ToListAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source, CancellationToken cancellationToken)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return GetQueryableWrapper(source).ToListAsync(cancellationToken);
    }

    public static Task&lt;TSource&gt; FirstAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source)
    {
        return FirstAsync(source, CancellationToken.None);
    }

    public static async Task&lt;TSource&gt; FirstAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source, CancellationToken cancellationToken)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        cancellationToken.ThrowIfCancellationRequested();

        using (var enumerator = source.GetAsyncEnumerator())
        {
            if (await enumerator.MoveNextAsync(cancellationToken))
            {
                return enumerator.Current;
            }
        }

        throw new InvalidOperationException(""Sequence contains no elements"");
    }

    public static Task&lt;TSource&gt; FirstOrDefaultAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source)
    {
        return FirstOrDefaultAsync(source, CancellationToken.None);
    }

    public static async Task&lt;TSource&gt; FirstOrDefaultAsync&lt;TSource&gt;(this IDbAsyncEnumerable&lt;TSource&gt; source, CancellationToken cancellationToken)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        cancellationToken.ThrowIfCancellationRequested();

        using (var enumerator = source.GetAsyncEnumerator())
        {
            if (await enumerator.MoveNextAsync(cancellationToken))
            {
                return enumerator.Current;
            }
        }

        return default(TSource);
    }

    public static IDbAsyncEnumerable&lt;T&gt; AsDbAsyncEnumerable&lt;T&gt;(this IQueryable&lt;T&gt; source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var enumerable = source as IDbAsyncEnumerable&lt;T&gt;;
        if (enumerable == null)
        {
            throw new InvalidOperationException(string.Format(""The source IQueryable doesn''t implement IDbAsyncEnumerable{0}. Only sources that implement IDbAsyncEnumerable can be used for Entity Framework asynchronous operations. For more details see http://go.microsoft.com/fwlink/?LinkId=287068."", ""&lt;"" + typeof(T) + ""&gt;""));
        }
        return enumerable;
    }
    #endregion

    #region private member functions
    private static IQueryable&lt;TSource&gt; GetQueryableWrapper&lt;TSource&gt;(IDbAsyncEnumerable&lt;TSource&gt; source)
    {
        return new QueryableWrapper&lt;TSource&gt;(source);
    }
    #endregion
}</pre>
<p>Implementaci nebudu popisovat úplně do detailu, ale upozorním na několik použitých “triků”.</p>
<ul>
<li>V metodě <em>AsDbAsyncEnumerable</em> stačí původní dotaz na <em>IDbAsyncEnumerable&lt;T&gt;</em> pouze přetypovat, protože původní dotaz již rozhraní <em>IDbAsyncEnumerable&lt;T&gt;</em> v EF automaticky implementuje. Mimochodem tato metoda je přesně takto implementovaná i přímo v <em>QueryableExtensions</em>, tam je ale pouze private. 
<li>V metodě <em>ToListAsync</em> potřebujeme zavolat původní implementaci této metody z EF. Protože ta je ale deklarovaná na <em>IQueryable&lt;T&gt;</em>, pomůžeme si tak, že nad naším <em>IDbAsyncEnumerable&lt;T&gt;</em> uděláme vlastní wrapper, který implementuje jak <em>IQueryable&lt;T&gt;</em> tak <em>IDbAsyncEnumerable&lt;T&gt;</em>. Implementace členů z <em>IQueryable&lt;T&gt;</em> stačí přitom prázdná, protože tu původní metoda <em>ToListAsync</em> nepoužívá (a my víme, že pro nic jiného wrapper použit nebude). Implementace <em>IDbAsyncEnumerable&lt;T&gt; </em>je nutná z toho důvodu, protože původní metoda <em>ToListAsync</em> provádí přetypování aktuálního objektu na <em>IDbAsyncEnumerable&lt;T&gt;</em>. Tato implementace pak ale jen deleguje volání <em>GetAsyncEnumerator</em> na původní wrappovaný objekt. 
<li>Metoda <em>FirstAsync</em> (resp. <em>FirstOrDefaultAsync</em>) musí být implementována obdobně jako implementace v LINQ to objects (ve třídě <a href=""https://msdn.microsoft.com/en-us/library/system.linq.enumerable(v=vs.110).aspx"" target=""_blank""><em>Enumerable</em></a>), ale “asynchronně” nad objektem <em>IDbAsyncEnumerable&lt;T&gt;</em>. Původní implementaci z EF zde použít nelze, protože ty fungují nad queryable a modifikují expression, který se překládá do SQL, což není to co my tady potřebujeme.</li></ul>
<h3>Výsledná asynchronní verze</h3>
<p>S použitím naší pomocné třídy <em>DbAsyncEnumerableExtensions</em> můžeme nyní již jednoduše dokončit asynchronní verzi původního příkladu.</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">public static class UzivateleService
{
    #region action methods
    public static async Task&lt;IList&lt;Models.Uzivatel&gt;&gt; GetUzivatele(bool zahrnoutVyrazene, CancellationToken cancellationToken)
    {
        using (var context = new DataContext())
        {
            var uzivatele = await GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziPlatnostDo == null || zahrnoutVyrazene)).ToListAsync(cancellationToken);
            return uzivatele;
        }
    }

    public static async Task&lt;Models.Uzivatel&gt; GetUzivatel(int IDUzivatele, CancellationToken cancellationToken)
    {
        using (var context = new DataContext())
        {
            return await GetUzivatele(context.Uzivatele.Where(u =&gt; u.uziIDUzivatele == IDUzivatele)).FirstOrDefaultAsync(cancellationToken);
        }
    }
    #endregion

    #region private member functions
    private static IDbAsyncEnumerable&lt;Models.Uzivatel&gt; GetUzivatele(IQueryable&lt;Uzivatel&gt; source)
    {
        var uzivatele = from result in
                            (from u in source
                            orderby u.uziPrijmeni, u.uziJmeno
                            select new
                            {
                                IDUzivatele = u.uziIDUzivatele,
                                Zapsano = u.uziZapsano,
                                Jmeno = u.uziJmeno,
                                Prijmeni = u.uziPrijmeni,
                                UzivatelLogin = u.UzivatelLogin,
                                Prihlaseni = (from prihlaseni in u.UzivatelPrihlaseni
                                                orderby prihlaseni.up_PosledniPrihlaseni descending
                                                select prihlaseni).FirstOrDefault(),
                                Vyrazen = u.uziPlatnostDo != null
                            }).AsDbAsyncEnumerable()
                        select new Models.Uzivatel()
                        {
                            IDUzivatele = result.IDUzivatele,
                            Zapsano = result.Zapsano,
                            Jmeno = result.Jmeno,
                            Prijmeni = result.Prijmeni,
                            PrihlasovaciJmena = string.Join("", "", result.UzivatelLogin.Select(o =&gt; o.ul_LoginName)),
                            PosledniPrihlaseni = result.Prihlaseni == null ? (DateTime?)null : result.Prihlaseni.up_PosledniPrihlaseni,
                            PosledniPrihlaseniPocitac = result.Prihlaseni == null ? null : result.Prihlaseni.up_PosledniPrihlaseniPocitac,
                            PosledniPrihlaseniOS = result.Prihlaseni == null ? null : result.Prihlaseni.up_PosledniPrihlaseniOS,
                            Vyrazen = result.Vyrazen
                        };

        return uzivatele;
    }
    #endregion
}</pre>
<p>Závěrem pouze upozorním, že řešení tohoto problému jsem nikde v oficiálních ani neoficiálních zdrojích nenašel, a musel jsem ho kompletně vymyslet a implementovat sám. Z tohoto důvodu přivítám k mojí implementaci jakékoliv vaše podměty.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8462, 15, CAST(N'2015-07-31T09:18:31.487' AS DateTime), N'Nastavení buildu serveru pro VS 2015 (MSBuild v 14.0)', N'<p><em>Pokud máme aplikaci ve VS 2015, ve které používáme nové možnosti jazyka <strong>C# 6.0</strong>, tak potřebujeme projekt kompilovat novým Roslyn kompilátorem. Na vývojovém počítači to problém není, ale na build serveru to problém být může. V mém případě provozujeme na build serveru <strong>TFS 2013</strong> Update 5 (protože TFS 2015 je zatím pouze RC 2, tak jsme ho ještě neinstalovali).</em></p> <p><em>Jak tedy přinutit, aby TFS 2013 build server používal nový kompilátor, přesněji řečeno, aby <strong>Build Agent spustil kompilaci pomoci</strong> <strong>MSBuild v 14.0</strong>?</em></p> <h3>Instalace <em><strong>.targets</strong></em> souborů</h3> <p>Na build server musíme <strong>nainstalovat</strong> plné <strong>Visual Studio 2015</strong> a to například i s komponentou Web Developer Tools, pokud buildujeme webovou aplikaci. Tím se nám teprve na build server nahrají potřebné <em><strong>.targets</strong></em> soubory, umístěné v cestě:<br><em>C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0<br></em>(Instalací samotného <em>mu_microsoft_build_tools_2015_x86_x64_6846132.exe</em> se tam nedostanou.)</p> <h3>Nastavení <strong>MSBuild</strong></h3> <p>Musíme přepnou <strong>MSBuild na verzi 14.0</strong>, aby se použil kompilátor <strong>C# 6.0</strong> tj.:<br><em>C:\Program Files (x86)\MSBuild\14.0\bin\csc.exe</em></p> <p>Na internetu nejspíše jako já najdete, že je nutné v Build definici v záložce Process nastavit ve vlastnosti <strong><em>MSBuild Arguments</em></strong> parametry ""<em>/tv:14.0 /p:VisualStudioVersion=14.0</em>""<br>Můžete to zkusit. Tím se sice kompilace již bude provádět pomoci nového MSBuild, ale v mém případě tudy cesta nevedla, protože se mi objevila následující chyba:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">error MSB4025: The project file could not be loaded. Could not find file ''C:\Builds\...Project.csproj.metaproj''.</pre>
<p>O co se jedná? Já mám totiž v build definici jako zdroj kompilace <strong>Solution (.sln)</strong>.<strong> </strong>V případě pouze projekt souboru (.csproj) by to asi prošlo.<br>V případě solution si ale MSBuild právě vytváří soubory <em><strong>.csproj.metaproj</strong></em> na udržování pořadí kompilace projektů,<br>no a z nějakého důvodu je při tomto spuštění přes build agenta tvoří pouze do paměti, a pak je na disku v uvedené cestě nenajde.</p>
<p>Zajímavé bylo, že když jsem na build serveru spustil MSBuild ručně z command line, tak se<em>.csproj.metaproj</em> na disk vytvořili a build prošel.</p>
<h3>Řešení pro kompilaci Solution</h3><br>
<p>Na parametry <em>MSBuild Arguments</em> v definici tedy zapomeňte. Skoro náhodou jsem ale objevil následující řešení.</p>
<p>Změníme <strong>MSBuild šablonu</strong>, kterou naše build definice používá (já již používal svojí vlastní upravenou).<br>Tato šablona představuje Workflow diagram (.xaml) celého build procesu. A jedna aktivita, která zde je, je právě volání MSBuild. Protože aktivit je tam opravdu hodně, je jednodušší soubor editovat přímo v textu než v designeru.</p>
<p>Najděte aktivitu <em><strong>mtbwa:MSBuild</strong></em> například:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;mtbwa:MSBuild CommandLineArguments=""[String.Format(&amp;quot;/p:SkipInvalidConfigurations=true {0}&amp;quot;, MSBuildArguments)]"" Configuration=""[platformConfiguration.Configuration]"" DisplayName=""Run MSBuild for Project"" GenerateVSPropsFile=""[True]"" MaxProcesses=""[If (MSBuildMultiProc, 0, 1)]"" OutDir=""[BinariesDirectory]"" Platform=""[platformConfiguration.Platform]"" Project=""[localBuildProjectItem]"" Targets=""[New String() { &amp;quot;Clean&amp;quot; }]"" TargetsNotLogged=""[New String() {&amp;quot;GetNativeManifest&amp;quot;, &amp;quot;GetCopyToOutputDirectoryItems&amp;quot;, &amp;quot;GetTargetPath&amp;quot;}]"" ToolPlatform=""[MSBuildPlatform]"" Verbosity=""[Verbosity]"" /&gt;</pre>
<p>a přidejte nastavení vlastnosti <em><strong>ToolVersion=""14.0""</strong></em>:</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;mtbwa:MSBuild CommandLineArguments=""[String.Format(&amp;quot;/p:SkipInvalidConfigurations=true {0}&amp;quot;, MSBuildArguments)]"" Configuration=""[platformConfiguration.Configuration]"" DisplayName=""Run MSBuild for Project"" GenerateVSPropsFile=""[True]"" MaxProcesses=""[If (MSBuildMultiProc, 0, 1)]"" OutDir=""[BinariesDirectory]"" Platform=""[platformConfiguration.Platform]"" Project=""[localBuildProjectItem]"" Targets=""[New String() { &amp;quot;Clean&amp;quot; }]"" TargetsNotLogged=""[New String() {&amp;quot;GetNativeManifest&amp;quot;, &amp;quot;GetCopyToOutputDirectoryItems&amp;quot;, &amp;quot;GetTargetPath&amp;quot;}]"" ToolPlatform=""[MSBuildPlatform]"" ToolVersion=""14.0"" Verbosity=""[Verbosity]"" /&gt;</pre>
<p>Já tuto aktivitu v souboru měl 2x, přidání tedy proveďte na všech místech.<br>Pozor, jedná se o string hodnotu, hodnota musí být celé ""<strong>14.0</strong>"", nestačí ""14"" (jak se nejprve povedlo mě).</p>
<p>Šablonu uložte a proveďte checkin do TFS.</p>
<p>Nyní při spuštění build definice s touto šablonou již kompilace projde v pořádku.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8464, 15, CAST(N'2015-08-10T11:44:17.633' AS DateTime), N'Ověřování webové aplikace pomoci klientských certifikátů', N'<p><em>Jedna z možností, jak zabezpečit přístup k webové aplikaci, je ověřování pomoci klientského certifikátu. Ukážeme si postup jak toto ověření nastavit. Postup je možné použít nejen k ověření celé webové aplikace, ale i například k Web API rozhraní.</em></p> <p>Nejprve k mému scénáři použití. Zákazník požadoval zpřístupnit personální data třetím stranám. Zhotovil jsem a vypublikoval webovou aplikaci, která pomoci HTTP handleru data poskytuje. Aplikace bude používat zabezpečený kanál pomoci HTTPS. Je ale více způsobů, jak zajistit ověření přístupu k této aplikaci. Windows (a ani basic) autentizace zde použít nešla, protože webová aplikace je provozovaná na počítači, kde nemáme přístup ke správě Windows uživatelů (jak doménových tak lokálních). Druhou možností by bylo použít Forms autentizaci. Když nepočítám to, že by aplikace nejspíše musela mít nějakou registraci a správu uživatelů, tak hlavní nevýhodou by bylo to, že by přístup vyžadoval zadávání loginu a hesla do přihlašovacího formuláře. Vzhledem k tomu, že k aplikací poskytovaným datům budou někteří klienti přistupovat nejspíše nějakým automatizovaným způsobem, je zadávání ověření do formuláře nevyhovující.</p> <p>Protože klientů je relativně malý počet, rozhodl jsem se řešit přístup k aplikaci pomoci klientských certifikátů, a to tak, že každému klientovi, kterému chceme povolit přístup k datům, vygenerujeme vlastní certifikát. Tím zároveň budeme moci podle použitého certifikátu při přihlášení zjistit, který klient se přihlásil, případně podle toho zabezpečit přístup pouze k některým datům.</p> <p>A nyní již k tomu jak jsem postupoval.</p> <p><strong>1) CA certifikát</strong></p> <p>Všechny certifikáty, které budeme klientům vytvářet budou vygenerované ze samostatné <strong>certifikační autority CA</strong>. Tu nemusíme nijak shánět nebo instalovat, pro tento scénář nám postačí pro autoritu pouze vygenerovat její vlastní certifikát. K tomu použijeme utilitu <strong>makecert.exe</strong> (dostupná ve <a href=""http://www.microsoft.com/en-us/download/details.aspx?id=8279"" target=""_blank"">Windows SDK</a>). Příkaz pro <strong>vygenerování certifikátu CA</strong> je následující: </p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">rem Vytvoření CA certifikátu
makecert -r -pe -n ""CN=WebAccessCA"" -ss CA -a sha1 -sky signature -cy authority -sv WebAccessCACert.pvk WebAccessCACert.cer</pre>
<p>Při spuštění budeme dotázání na heslo (passphrase) pro vygenerování klíče. Já jsem si heslo vygeneroval pomoci <a href=""http://chaos.aspnet.cz"" target=""_blank"">http://chaos.aspnet.cz</a>. Vzniknou nám soubory certifikátu <em><strong>WebAccessCACert.cer</strong></em> a privátního klíče <em><strong>WebAccessCACert.pvk</strong></em>.</p>
<p>Na počítači, kde webovou aplikaci hostujeme, naimportujeme soubor certifikátu certifikační autority <em>WebAccessCACert.cer</em> do Windows úložiště certifikátu <b>Trusted Root Certification Authorities</b> (Důvěryhodné kořenové certifikační úřady) pro <b>Počítač – Local Computer</b>.</p>
<p><strong>2) Klientský certifikát</strong></p>
<p>Druhým krokem bude vygenerování klientského certifikátu. Tento krok budeme používat opakovaně, při generování dalšího certifikátu novému klientovi. Opět použijeme makecert, navíc ale ještě pomoci <strong>pvk2pfx.exe</strong> převedeme soubory cer a pvk na soubor pfx.</p><pre class=""brush: text; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">rem Vytvoření Client certifikátu
makecert -pe -n ""CN=Klient1"" -a sha1 -sky exchange -eku 1.3.6.1.5.5.7.3.2 -ic WebAccessCACert.cer -iv WebAccessCACert.pvk -sv Klient1ClientCert.pvk Klient1ClientCert.cer

rem Převod souborů klíče pvk a certifikátu cer na pfx
pvk2pfx -pvk Klient1ClientCert.pvk -spc Klient1ClientCert.cer -pfx Klient1ClientCert.pfx -po MJAZWB!eAGoL
</pre>
<p>Při spuštění příkazu zadáme jak nové heslo pro klíč klientského certifikátu (opět můžeme vygenerovat), tak i původní heslo ke klíči CA certifikátu (Issuer Signature). Všimněte si dále v příkazu určení subjectu certifikátu (například CN=Klient1), podle něho pak budeme klienty rozeznávat.</p>
<p>Danému klientovi pak předáme soubor certifikační autority <em><strong>WebAccessCACert.cer</strong></em> a soubor klientského certifikátu ve formátu pfx <strong>Klient1ClientCert.pfx</strong> (a heslo k němu). 
<p>Na počítači klienta (nebo na vývojovém / testovacím počítači) poté provedeme tyto kroky:</p>
<p>1. Nainstalujeme certifikát <b><strong>Klient1ClientCert</strong>.pfx</b> do Windows úložiště certifikátu <b>Personal</b> (Osobní) pro <b>Uživatele – Current User</b>.<br>2. Nainstalovat certifikát <b><em><strong>WebAccessCACert</strong></em>.cer</b> do Windows úložiště certifikátu <b>Trusted Root Certification Authorities</b> (Důvěryhodné kořenové certifikační úřady) pro <strong>Počítač – Local Computer</strong>.</p>
<p><strong>3. Nastavení IIS</strong> 
<p>Provedeme potřebné nastavení pro klientský certifikát na IIS. Předpokladem je dále nastavený HTTPS binding. V IIS manageru vybereme naší aplikaci a zvolíme <strong>SSL Settings</strong>. 
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Ovovn-webov-aplikace-pomoci-klienskch-ce_BE58/image_d9270812-6384-4c3a-a779-663e6dca4bf8.png"" width=""675"" height=""170""> 
<p>Zapneme vyžadování SSL a dále zvolíme <strong>Require</strong> v nastavení <strong>Client certificates</strong>. 
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Ovovn-webov-aplikace-pomoci-klienskch-ce_BE58/image_16e776c5-405f-4e88-b84c-97eed87f00ce.png"" width=""472"" height=""201""> 
<p><strong>4. Ověření certifikátu v aplikaci</strong> 
<p>Pokud k aplikaci nyní budeme přistupovat z prohlížeče, budeme vyzvání k vybrání certifikátu, nabídnou se všechny validní certifikáty z Personal úložiště uživatele (a sem jsme právě klientský certifikát nainstalovali). Vybraný certifikát je v aplikaci dostupný pomoci vlastnosti <strong>ClientCertificate</strong> na <strong>Request</strong> objektu a to jestli byl certifikát zadán je možné zjistit vlastností <strong>IsPresent</strong>. Aplikace je nyní ale přístupná po vybrání jakéhokoliv platného certifikátu, co tedy dále budeme potřebovat bude zkontrolovat, zda se jedná o náš certifikát, který jsme dali klientovi. 
<p>V mém případě jsem kontrolu umístil přímo do handleru, který generuje data, ale bylo by možné jí umístit například do global.asax nebo samostatného modulu pro kontrolu všech requestů. Kód bude následující:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">if (!HttpContext.Current.Request.ClientCertificate.IsPresent)
{
    context.Response.StatusCode = 401;
    context.Response.StatusDescription = ""Unauthorized"";
    context.Response.End();
    return;
}

var cert = new X509Certificate2(HttpContext.Current.Request.ClientCertificate.Certificate);
if (!this.IsValidCertificate(cert))
{
    context.Response.StatusCode = 401;
    context.Response.StatusDescription = ""Unauthorized"";
    context.Response.End();
    return;
}

var identity = new GenericIdentity(cert.Subject, ""ClientCertificate"");
var principal = new GenericPrincipal(identity, null);
Thread.CurrentPrincipal = principal;
HttpContext.Current.User = principal</pre>
<p>Po ověření certifikátu je vytvořena Identita <strong>GenericIdentity</strong>, kde jako jméno použijeme Subject certifikátu. 
<p>Ještě nám zbývá implementovat metodu <strong>IsValidCertificate</strong>. Protože ale klientských certifikátu může být více, nemůžeme kontrolovat přímo předaný certifikát. Místo toho zkontrolujeme, zda byl certifikát vydaný z naší certifikační autority. Jinými slovy zkontrolujeme, že kořenový certifikát je náš <em><strong>WebAccessCACert.cer</strong>.</em></p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: false;"">private bool IsValidCertificate(X509Certificate2 certificate)
{
    var chain = new X509Chain();
    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

    if (!chain.Build(certificate))
    {
        return false;
    }

    var caCert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;

    return caCert.Thumbprint.Equals(Properties.Settings.Default.CACertificateThumbprint, StringComparison.OrdinalIgnoreCase);
}</pre>
<p>Pomoci <strong>X509Chain</strong> se dostaneme k nadřazeným certifikátům. Pro kontrolu CA certifikátu jsem použil porovnání podle jeho <strong>Thumbprint</strong>, který jsem si zadal do konfigurace (přes generované Settings). 
<p>Tím je nyní handler dostupný pouze při přístupu certifikáty klientů vytvořeném v kroku 2.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8465, 15, CAST(N'2015-08-18T16:57:00.000' AS DateTime), N'Nastavení detailního chybového výpisu v aspnet5 při 500 Internal Server Error', N'<p><strong><em><font color=""#ff0000"">V ASP.NET 5 byl od beta8 změněn hosing model na IIS. Již se nepoužívá ""Helios"" IIS host, místo toho se využívá ASP.NET 5 </font></em></strong><a href=""https://github.com/aspnet/KestrelHttpServer/"" target=""_blank""><strong><em><font color=""#ff0000"">Kestrel server</font></em></strong></a><strong><em><font color=""#ff0000""> v konfiguraci s IIS </font></em></strong><a href=""http://azure.microsoft.com/en-us/blog/announcing-the-release-of-the-httpplatformhandler-module-for-iis-8/"" target=""_blank""><strong><em><font color=""#ff0000"">HttpPlatformHandler</font></em></strong></a><strong><em><font color=""#ff0000""> modulem (více </font></em></strong><a href=""https://github.com/aspnet/Announcements/issues/69"" target=""_blank""><strong><em><font color=""#ff0000"">zde</font></em></strong></a><strong><em><font color=""#ff0000"">). Proto je tento článek deprecated a popsané řešení již nefunguje.</font></em></strong></p> <p>Pokud publishujete <strong>aspnet5 webovou aplikaci do IIS</strong> na nějakém vzdáleném serveru, například na <strong>Microsoft Azure Web Apps</strong>, a máte chybu při vlastním startu aplikace, dostanete zobrazenou pouze nic neříkající <strong>Oops, 500 Internal Server Error</strong> hlášku.</p> <p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Nastaven-detailnho-chybovho-vpisu-pi_CFEB/image_3.png"" width=""557"" height=""202""></p> <h3>Jak se dozvědět více?</h3> <p>Řešení tohoto problému je následující:</p> <p>Najít používaný soubor <strong>Web.config</strong> v adresáři <strong>wwwroot</strong> webu, v případě Web App můžeme použít přístup přes FTP. Web.config pro aspnet5 hostované v IIS vypadá typicky takto:</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;configuration&gt;
  &lt;appSettings&gt;
    &lt;add key=""bootstrapper-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""runtime-path"" value=""..\approot\runtimes"" /&gt;
    &lt;add key=""dnx-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""dnx-clr"" value=""clr"" /&gt;
    &lt;add key=""dnx-app-base"" value=""..\approot\src\MyApp"" /&gt;
  &lt;/appSettings&gt;
  &lt;system.web&gt;
    &lt;httpRuntime targetFramework=""4.5.1"" /&gt;
  &lt;/system.web&gt;
&lt;/configuration&gt;</pre>
<p>Do něho je nutné doplnit nastavení klíče <strong>ASPNET_DETAILED_ERRORS</strong> na hodnotu <strong>true</strong>.</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: true; highlight: [8-10]; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;configuration&gt;
  &lt;appSettings&gt;
    &lt;add key=""bootstrapper-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""runtime-path"" value=""..\approot\runtimes"" /&gt;
    &lt;add key=""dnx-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""dnx-clr"" value=""clr"" /&gt;
    &lt;add key=""dnx-app-base"" value=""..\approot\src\DC3"" /&gt;
    &lt;!-- This will turn on detailed errors when deployed to remote servers --&gt; 
    &lt;!-- This setting is not recommended for production --&gt; 
    &lt;add key=""ASPNET_DETAILED_ERRORS"" value=""true"" /&gt;
  &lt;/appSettings&gt;
  &lt;system.web&gt;
    &lt;httpRuntime targetFramework=""4.5.1"" /&gt;
  &lt;/system.web&gt;
&lt;/configuration&gt;</pre>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8468, 18, CAST(N'2015-09-18T09:55:18.777' AS DateTime), N'Azure Mobile Services Backend, díl 1.', N'<p>Každý druhý programátor se již asi setkal s tím, že po vytvoření aplikace na jednu platformu, byla potřeba portace na platformu druhou. Někteří možná mohli narazit na to, že původní technologie využitá pro cloudový backend - ať už šlo jen o pár dat v SQL databázi, či rozsáhlé datové struktury-, nebyla dostupná na platformě nové.  <p>Microsoft pár let zpátky spustil službu nesoucí jméno ""Azure Mobile Services"", která je dostupná na většinu (ne-li všechny) dosavadní platformy, čímž nám v mnohém ulehčuje práci a dělá prožitek z využívání Microsoftích technologiích zase o něco příjemnějším.  <p>S Azure M.S. jsem se poprvé setkal více než před dvěma lety. Od té doby prošla tato technologie mnohými úpravami, ať už šlo o službu jako takovou, nebo jen dokumentaci. Vyzkoušel jsem si, jaké to bylo programovat se skoro nulovou dokumentací, stejně jako později již s krásnou a přehlednou dokumentací, spoustou návodů apod.  <p>Od této mini-série nečekejte návod v podobě dokumentace. Tato mini-série by měla poskytnout pomocnou ruku a úvod do problematiky Azure M.S. mírně pokročilým programátorům, kteří přemýšlí, že jejich například již napsanou aplikaci přesunou do nové cloudové sféry a otevřou si tak dveře k lehčí portaci na ostatní platformy, nebo přesvědčit zkušené programátory, že to má cenu.  <p>Po absolvování mého úvodu do Azure M.S., by měl být čtenář schopný jít a začít řešit svůj vlastní cloudový backend. Měl by mít představu o čem práce s Azure M.S. je, jak si poradit s problémy a popřípadě být schopen vymyslet vlastní řešení problémů, se kterými se může setkat. Nezůstaneme samozřejmě jen u teorie, ale ukážeme si nějaké příklady kódu z praxe.  <p>Toť pro dnešní díl vše, už jen připojím užitečné linky, které by se mohly hodit a popřeji hodně štěstí a málo bugů!  <p>Dokumentace / tutoriál Azure Mobile Services - <a href=""https://azure.microsoft.com/en-us/documentation/articles/mobile-services-javascript-backend-windows-store-dotnet-get-started/"">https://azure.microsoft.com/en-us/documentation/articles/mobile-services-javascript-backend-windows-store-dotnet-get-started/</a>  <p>Azure one-month trial - <a href=""https://azure.microsoft.com/en-us/pricing/free-trial/"">https://azure.microsoft.com/en-us/pricing/free-trial/</a>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8471, 18, CAST(N'2015-10-23T17:14:52.767' AS DateTime), N'Azure mobile services backend, díl 2.', N'<p>V dnešním dílu naší mini-série se budeme věnovat a obeznámíme se s problematikou tzv. ‘‘soft delete‘‘, offline synchronizace a řešení konfliktů. Kromě toho co vůbec Azure M.S. je a k čemu se dá využít. V příštím díle se již dostaneme k ukázce kódu a k řešení problematiky Azure M.S. v praxi. <p>Azure M.S. má velkou škálu možností, které nabízí a víceméně záleží jen na programátorovi, jak se k dostupným technologiím postaví a využije je. V oficiálních tutoriálech se využívá jako názorný příklad tzv. ‘‘ToDo List‘‘, což je vhodně zvolený příklad k vysvětlení a obeznámení se základními principy. Tam to ale ovšem nekončí, Azure M.S. se dá využít i ke komplexnímu aplikačnímu backendu. <p>Řekněme, že programujete jednoduchý poznámkovač s cloudovým backendem. Pravděpodobně se nespokojíte s tím, že vaše aplikace běží jen s funkčním připojením k internetu. V tuto chvíli přichází na řadu offline synchronizace. No a jak to už bývá, nic není tak jednoduché a při implementaci offline synchronizace si začnete uvědomovat, že budete potřebovat více. V tu chvíli přichází na řadu tzv. ‘‘soft delete‘‘ a řešení konfliktů. <p>Offline synchronizace se v našem případě řeší implementací lokální databáze. Synchronizace se posléze řeší pushem a pullem (ukážeme si v příštím díle). Při takovém chování aplikace, ale samozřejmě může nastat problém se synchronizací a je potřeba vyřešit konflikty. <p>Představte si, že jedna aplikace se pokusí nahrát změnu nějakého zaznámu a ve stejný okamžik se o to samé, ale nyní již se zastaralou verzí záznamu pokusí aplikace druhá. V tuto chvíli nám aplikace díky Azure knihovnám vyhodí výjimku a je na nás, jak ji vyřešíme. Samozřejmě to nemusíme řešit celé sami od piky. Jednoduše vytvoříme třídu, která nám to bude řešit, podědíme IMobileServiceSyncHandler, implementujeme nezbytné třídy a doupravíme si kód, podle vlastních představ. <p>Další problém nastává v mazání záznamů z aplikace, tedy v pozadí z databáze. K tomu nám slouží již zmínění ‘‘soft delete‘‘. Je to volitelný sloupeček v naší databázi. Funguje to tak, že pokud potom smaži záznam, fyzicky se smazání neprovede, jen se nastaví hodnota _deleted sloupečku na true a já se pak můžu při vytahování dat z databáze rozhodnout, jestli chci zahrnout i takové záznamy. V případě že ne, se záznamy z lokální databáze smažou a zůstanou jen v té na Azure. Takové záznamy nám můžou sloužit k jednoduchému zotavení v případě mylného, či neúmyslného smazání uživatelem. Je ale potřeba mysle také na to, že nám zabírají místo v databázi a měli bychom je občas pročistit. <p>Toť pro dnešní díl vše. Jak jsem již říkal, do příštího dílu vám slibuji praktické ukázky, takže se posuneme od úvodní omáčky k hlavnímu chodu. <p>Budu se těšit a jako vždy přeji hodně štěstí a málo bugů! :)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8476, 18, CAST(N'2015-11-17T19:35:26.613' AS DateTime), N'AZURE MOBILE SERVICES BACKEND, DÍL 3.', N'<p>Jak jsem slíbil, dnes se podíváme na praktické použití. Jako příklad nám poslouží 2 třídy, jež jsem programoval nějaký čas zpátky. Třída “DatCore.cs“ a třída “SyncHandler.cs“. Tyto 2 třídy slouží v aplikaci pro cloudové poznámky (synchronizace mezi zařízeními s Windows 8.1). V DatCore nalezneme veškerou potřebnou algoritmizaci a SyncHandler je jen pomocná třída pro řešení (jak název napovídá) kolizí apod. Třídy jsou ke stažení <a href=""http://uloz.to/xucvcJbq/datcore-zip"">zde</a>.  <p>Začněme třídou SyncHandler, která dědí od IMobileServiceSyncHandler interface. Vidíme v ní dva “virtual“ Tasky. Virtuální protože to dovoluje Entity Frameworku například tzv. lazy loading a zefektivňuje sledování změn. Na začátku si můžeme povšimnout třech globálních proměnných. Dva stringy a náš MobileServiceClient, jež inicializujeme a deklarujeme v App.xaml.cs</p> <p><br>(MobileServiceClient NasKlient = new MobileServiceClient(""https://projekt.azure-mobile.net/"", ""AppKey""); )</p> <p><br>ve třídě SyncHandler ho jen přebíráme a pracujeme s ním jako s odkazem (předání objektu v konstruktoru třídy). Dále veřejný, “virtual Task &lt;JObject&gt; OnPushCompleteAsync“ a privátní “async Task&lt;IUICommand&gt; ShowConflictDialog“, které předpokládám, není třeba vysvětlovat. Co nás zajímá nejvíce je veřejný “virtual async Task&lt;JObject&gt; ExecuteTableOperationAsync“. Tento task je to, co nám konflikty řeší. Tady si můžeme přizpůsobit chování algoritmu v případě, že se naskytne nějaký konflikt například, nebo můžeme (tak jak je to v tomto případě) dát na výběr uživateli (ve většině případů preferovaný postup). </p> <p>To by byla tedy kompletní třída SyncHandler, nyní se můžeme posunout ke tříde DatCore. Ta je mnou vytvořená třída, obstarávající algoritmus v pozadí, tedy takové jádro, core. Na začátku můžeme opět vidět dvě globální proměnné, naší tabulku a datovou kolekci objektů typu Table, tedy veškerý obsah z tabulky v jednom “listu“. Dále máme region s veřejnými metodami, které nám slouží k: počáteční inicializaci, obnově kolekce, či přidání / smazání / editace záznamu.  <p>Dále region s našemi Tasks. Zde můžeme vydět logiku přidávání / mazání / editace položek, či obnovu kolekce již v privátní podobě. Pozastavíme se nad Taskem “RefreshNoteItems“. Zde se inicializují naše _items. Protože nepotřebujeme vytvořit list položek z databáze všech uživatelů, filtrujeme výběr tzv. výrazem <b>λ </b>(lambda) jen na položky s ID našeho uživatele.  <p>Další region nám řeší offline synchronizaci. Máme tu task, který inicializuje naší lokální databázi, ve kterém i přiřazujeme naší SyncHandler třídu pro řešení konfliktů synchronizace. A task synchronizace, ve které nejdříve provedeme push a po sléze pull.  <p>Toť k dnešnímu dílu vše. Doufám, že se mi podařilo alespoň někomu vnést trochu světla do algoritmizace synchronizace cloudového backendu Azure M.S. a těším se na vaše ohlasy. Pokud něco potřebuje ještě víc osvětlení, nebo máte připomínky jakéhokoliv typu, nebojte se ozvat. Uvítám i kritiku a popřípadě článek ještě doupravím.  <p>V příštím, posledním díle se můžeme těšit na shrnutí a pár připomínek / prohlášení, které bych ještě rád udělal (popřípadě pokud bude třeba obsáhlejší vysvětlení).  <p>Rád bych také připomenul, že tato mini-série nemá sloužit jako návod k implementaci. Od toho tu jsou oficiální zdroje Azure (link v prvním <a href=""http://www.dotnetportal.cz/blogy/18/Daniel-Vittek/8468/Azure-Mobile-Services-Backend-dil-1-"">dílu</a>).  <p>Budu se těšit a jako vždy přeji hodně štěstí a málo bugů! :)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8477, 3, CAST(N'2015-11-19T14:10:52.523' AS DateTime), N'Prezentace a dema z přednášky Edge pro webové vývojáře', N'<p>Včera (18. listopadu) jsme měli s <a href=""http://www.dajbych.net"">Vaškem Dajbychem</a> přednášku na téma <strong>Edge pro webové vývojáře</strong>. Prezentaci a odkazy na dema si můžete stáhnout <a href=""http://www.dotnetportal.cz/Content/edge_dema.zip""><strong>zde</strong></a>.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8482, 18, CAST(N'2015-12-19T17:27:50.297' AS DateTime), N'azure mobile services backend, díl 4. - závěr', N'<p>Tímto dílem bych chtěl ukončit naší mini-sérii. Doufám, že jsem alespoň někomu nastínil tuto problematiku, či pomohl tuto technologii pochopit. Jsem rád, že se našel i někdo, kdo mě kontaktoval kvůli dodatečnému vysvětlení. Pokud by někdo měl další dotazy, kontaktovat mě můžete kdykoliv, s čímkoliv na danielvittek@outlook.cz <p>Někdo se ptal, proč dělám sérii na Azure Mobile Services, když aktuálnější jsou již Azure Mobile Apps. Nuže, s touto sérií jsem (alespoň imaginárně) začal již před delší dobou, kdy ještě byly Azure Mobile Services aktuální. V každém případě, tyto 2 technologie se od sebe liší jen minimálně. Koncept zůstal téměř stejný. Takže, kdo pochopil toto, nebude mít problém přejít na Azure Mobile Apps. <p>Jako vždy přeji hodně štěstí a málo bugů! :)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8483, 15, CAST(N'2016-01-04T16:53:33.427' AS DateTime), N'ed For Theme Detection (a66e367a-cf2c-4bf7-8566-f4236f0c9128 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)', N'<p>This is a temporary post that was not deleted. Please delete this manually. (da160037-5ee1-4a62-a910-4cb23b938130 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8484, 15, CAST(N'2016-01-04T16:58:05.180' AS DateTime), N'ed For Theme Detection (b309917a-44d8-42e7-9b8e-cf8af14f9dc8 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)', N'<p>This is a temporary post that was not deleted. Please delete this manually. (80ae1d1f-914c-4b6a-afc7-f2ce3ba3d994 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8485, 15, CAST(N'2015-08-18T16:57:00.000' AS DateTime), N'Nastavení detailního chybového výpisu v aspnet5 při 500 Internal Server Error', N'<p><strong><em><font color=""#ff0000"">V ASP.NET 5 byl od beta8 změněn hosing model na IIS. Již se nepoužívá ""Helios"" IIS host, místo toho se využívá ASP.NET 5 </font></em></strong><a href=""https://github.com/aspnet/KestrelHttpServer/"" target=""_blank""><strong><em><font color=""#ff0000"">Kestrel server</font></em></strong></a><strong><em><font color=""#ff0000""> v konfiguraci s IIS </font></em></strong><a href=""http://azure.microsoft.com/en-us/blog/announcing-the-release-of-the-httpplatformhandler-module-for-iis-8/"" target=""_blank""><strong><em><font color=""#ff0000"">HttpPlatformHandler</font></em></strong></a><strong><em><font color=""#ff0000""> modulem (více </font></em></strong><a href=""https://github.com/aspnet/Announcements/issues/69"" target=""_blank""><strong><em><font color=""#ff0000"">zde</font></em></strong></a><strong><em><font color=""#ff0000"">). Proto je tento článek deprecated a popsané řešení již nefunguje.</font></em></strong></p> <p>Pokud publishujete <strong>aspnet5 webovou aplikaci do IIS</strong> na nějakém vzdáleném serveru, například na <strong>Microsoft Azure Web Apps</strong>, a máte chybu při vlastním startu aplikace, dostanete zobrazenou pouze nic neříkající <strong>Oops, 500 Internal Server Error</strong> hlášku.</p> <p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportal.blob.core.windows.net/files/Windows-Live-Writer/Nastaven-detailnho-chybovho-vpisu-pi_CFEB/image_3.png"" width=""557"" height=""202""></p> <h3>Jak se dozvědět více?</h3> <p>Řešení tohoto problému je následující:</p> <p>Najít používaný soubor <strong>Web.config</strong> v adresáři <strong>wwwroot</strong> webu, v případě Web App můžeme použít přístup přes FTP. Web.config pro aspnet5 hostované v IIS vypadá typicky takto:</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: true; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;configuration&gt;
  &lt;appSettings&gt;
    &lt;add key=""bootstrapper-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""runtime-path"" value=""..\approot\runtimes"" /&gt;
    &lt;add key=""dnx-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""dnx-clr"" value=""clr"" /&gt;
    &lt;add key=""dnx-app-base"" value=""..\approot\src\MyApp"" /&gt;
  &lt;/appSettings&gt;
  &lt;system.web&gt;
    &lt;httpRuntime targetFramework=""4.5.1"" /&gt;
  &lt;/system.web&gt;
&lt;/configuration&gt;</pre>
<p>Do něho je nutné doplnit nastavení klíče <strong>ASPNET_DETAILED_ERRORS</strong> na hodnotu <strong>true</strong>.</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: true; highlight: [8-10]; html-script: false; light: false; ruler: false; smart-tabs: true; tab-size: 4; toolbar: true;"">&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;configuration&gt;
  &lt;appSettings&gt;
    &lt;add key=""bootstrapper-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""runtime-path"" value=""..\approot\runtimes"" /&gt;
    &lt;add key=""dnx-version"" value=""1.0.0-beta6"" /&gt;
    &lt;add key=""dnx-clr"" value=""clr"" /&gt;
    &lt;add key=""dnx-app-base"" value=""..\approot\src\DC3"" /&gt;
    &lt;!-- This will turn on detailed errors when deployed to remote servers --&gt; 
    &lt;!-- This setting is not recommended for production --&gt; 
    &lt;add key=""ASPNET_DETAILED_ERRORS"" value=""true"" /&gt;
  &lt;/appSettings&gt;
  &lt;system.web&gt;
    &lt;httpRuntime targetFramework=""4.5.1"" /&gt;
  &lt;/system.web&gt;
&lt;/configuration&gt;</pre>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8486, 15, CAST(N'2016-01-05T09:12:32.200' AS DateTime), N'Jak rozchodit 32-bit proces v IIS s instalovaným HttpPlatformHandler modulem', N'<p>Včera jsem řešil následující problém:<br>Na vývojovém počítači, kde mám nainstalovaný <strong>HttpPlatformHandler</strong> pro vývoj pro ASP.NET 5 mi nechodila v IIS starší aplikace, která potřebuje 32-bitový režim - aplikační pool s volbou <strong>Enable 32-Bit Application</strong>.</p> <p>Při spuštění aplikace se zobrazila chyba <strong>Service Unavailable</strong> a aplikační pool spadl. Více prozradil až eventlog, kde se objevila následující chyba:<br><em>The Module DLL C:\WINDOWS\system32\inetsrv\httpPlatformHandler.dll failed to load.&nbsp; The data is the error.</em></p> <p>Z toho se tedy dá usoudit, že problém je v tom, že se IIS pokouší načíst <strong>HttpPlatformHandler</strong> modul, který pod 32-bit modem nechodí.</p> <p>Vzpomněl jsem si, že podobný problém jsem kdysi řešil s 32-bit aplikačním poolem na počítači s instalovaným WSUS, kde chyběla jeho 32-bit verze dll souboru (suscomp.dll). Tam bylo potřeba 32-bit dll do IIS ručně donahrát.</p> <p>Zkusil jsem nainstalovat 32-bit verzi <strong>HttpPlatformHandler</strong>, ale instalace <em><strong>httpPlatformHandler_x86.msi</strong></em> pouze oznámí “<em>The 32-bit version of Microsoft Platform Handler 1.2 cannot be installed on a 64-bit edition of Microsoft Windows</em>”.</p> <p>Přišel jsem tedy na toto řešení:</p> <ul> <li>Instalaci <em><strong>httpPlatformHandler_x86.msi</strong></em> je potřeba ručně rozpakovat, já na to použil program <strong>7-zip</strong>.</li> <li>Rozpakovaný soubor <strong><em>HttpPlatformHandlerDll</em></strong> přejmenujeme na <em><strong>HttpPlatformHandler.dll</strong></em>.</li> <li>Soubor <strong><em>HttpPlatformHandler.dll</em></strong> nahrajeme do adresáře <strong>C:\Windows\SysWOW64\inetsrv</strong>.</li> <li>Zrestartujeme IISko příkazem <strong>iisreset</strong> z administrátorský command line. (Nahraná dll není naštěstí potřeba nijak registrovat.)</li></ul> <p>Po těchto krocích již 32-bit webová aplikace funguje.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8488, 18, CAST(N'2016-01-15T16:51:27.350' AS DateTime), N'MODERNÍ WPF APLIKACE A VIEW MODEL', N'<p>V poslední době jsem se na několika místech setkal s případem špatné implementace view modelu v moderních WPF aplikacích. Rozhodl jsem se to tedy formou tohoto článku vysvětlit.  <p>Ať už využíváte jakýkoliv objektový návrh aplikace, vždy se snažíte alespoň o základní rozdělení modelu, view a controleru (MVC). Hodně lidí momentálně přechází z vývoje starých WinForm aplikací na nové WPF (ať už desktopové, či Windows 8/8.1/10) a jsou trošku zmatení novým bindováním view modelu v jazyku XAML.  <p>Z jednoduché logické úvahy vyplývá, že někdy je potřeba umět k view modelu přistoupit jak z XAML části aplikace, tak z nějaké klasické C# třídy(.cs).  <p>Představme si třídu; klasický view model například s jedním label stringem a jednou visibilitou. Již několikrát jsem se setkal s případy, kdy se někdo potýkal s nefunkčností takto jednoduchého view modelu, i přesto že si byl na 100% jistý, že všechno udělal správně. V XAML části bylo správně nabindované rozhraní mezi view modelem a konkrétní komponentou a v logické části programu (z nějaké modelové třídy) byly správně naplňované hodnoty view modelu, ale i přesto to prostě ""záhadně"" nefungovalo.  <p>Problém je u 90% vždy stejný. Při vytváření proměnné našeho view modelu v XAML části totiž (logicky) dochází k vytvoření nové instance objektu, a to je třeba si uvědomit. Nemůžeme tedy po sléze (například v MainPage.xaml.cs) vytvářet novou instanci a očekávat, že se nám po vyplnění hodnot ve view něco změní.  <p>Řešení se zdá být celkem jednoduché. Řeknete si, proč neudělat ze třídy ViewModel singleton (jedináčka). Takové řešení by bylo nejspíše nejlepší a nejelegantnější, bohužel je ale nemožné implementovat. V XAML části totiž nemůžeme přistoupit ke statické metodě ""getInstance"". Problém se tedy musí vyřešit jinak.  <p>Podle mě je nejlepší způsob si buď vytvořit XAML globální proměnnou v App.xaml a tak k našemu view modelu mít přístup odkudkoliv, nebo vytvářet náš view model globálně v rámci jen našeho jednoho view (MainPage.xaml) a inicializovat v kódové části view (MainPage.xaml.cs). Tímto způsobem dosáhneme jednoduchého přenášení jedné instance view modelu mezi třídami, dodržíme jednoduchý a elegantní způsob bindování a neporušíme (téměř) žádná pravidla i nejzákladnějšího objektového návrhu MVC.  <p>&nbsp; XAML:<br>&lt;Window.Resources&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;ViewModel:MainWindowViewModel x:Key=""MainWindowViewModel"" /&gt;<br>&lt;/Window.Resources&gt;</p> <p>C#:<br>public static MainWindowViewModel MainWindowViewModel = Resources[""MainWindowViewModel""] as MainWindowViewModel;</p> <p>Doufám, že byl tento malý tutoriálek alespoň někomu užitečný a jako vždy, hodně štěstí a málo bugů! :)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8491, 18, CAST(N'2016-02-16T13:23:36.637' AS DateTime), N'IN-APP PRODUCT, C#, WINRT, DÍL 1.', N'<p>V dnešním díle se podíváme na nákupy z aplikací. Rozdělíme si je do dvou základních kategorií a vysvětlíme, jak nákupy z aplikací simulovat.  <p>Nákupy z aplikací jsou pro malé -ale i nemalé- vývojáře jednou z nejlepších cest, jak na své práci vydělat nějaké peníze. V dnešní době je to dokonce jediný způsob, jak na určitých platformách na aplikaci něco vydělat. Určitě znáte spousty aplikací a her na mobilní, či tabletové platformě, které obsahují spoustu reklam, které si za malý poplatek můžete vypnout, nebo exkluzivní produkty, často je to například nová úroveň hry. Klasický příklad jak můžeme vydělat na aplikaci a vyhnout se pirátům (ano stále jsou tu tací, kteří nezaplatí za sebelepší aplikaci ani cent a radši jí ukradnou).  <p>Máme 2 kategorie produktů. První kategorie “consumable“ jsou například ve hrách speciální mince, za které si uživatel může nakupovat malé vylepšení (například design postavičky). Do druhé kategorie spadají tzv. “durable“, věc kterou si jednou koupíme a můžeme jí využívat třeba na vždy (například speciální zbraň pro postavičku ve hře).  <p>Ke konkrétnímu kódu a podrobnější problematice se dostaneme v příštím díle, ale rád bych ještě zmínil několik drobností.  <p>Je potřeba si uvědomit, že na WinRT (na rozdíl od Silverlight) nemáme možnost testovat nákupy pomocí knihovny “Mock IaP“ a v lokální debug verzi nám nákupy NEBUDOU FUNGOVAT! Máme tedy na výběr ze dvou možností, jak implementaci IaPs otestovat. Já osobně preferuji kombinovat obě varianty, ale pokud jde ale jen o malou aplikaci s malou zákaznickou základnou, vystačíte si i s první variantou.  <p>První varianta testování využívá tzv. “CurrentAppSimulator“. Funguje to tak, že namísto aby aplikace získávala informace o produktech, které vlastníme a které jsou k dispozici ke koupi ze Store, získává je z lokálního XML souboru.  <p>Implementace je vskutku jednoduchá. Doporučuji kód rozdělit pomocí ‘#if DEBUG‘ podmínek. Takže například LicenceInfo se bude načítat takto:<br><br><code>#if DEBUG<br>var _licInfo = CurrentAppSimulator.LicenseInformation;<br>#else<br>var _licInfo = CurrentApp.LicenseInformation;<br>#endif</code><br></p> <p>Simulátor inicializujeme takto:<br><br><code>StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync(""Data"");<br>StorageFile proxyFile = await proxyDataFolder.GetFileAsync(""WindowsStoreProxy.xml"");<br>await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);</code></p> <p><br>XML soubor jako takový vypadá například takto:<br><br><code>&lt;?xml version=""1.0"" encoding=""utf-16"" ?&gt;<br>&lt;CurrentApp&gt;<br>&nbsp; &lt;ListingInformation&gt;<br>&nbsp;&nbsp;&nbsp; &lt;App&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;AppId&gt;988b90e4-5d4d-4dea-99d0-e423e414ffbc&lt;/AppId&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;LinkUri&gt; &lt;/LinkUri&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;CurrentMarket&gt;en-gb&lt;/CurrentMarket&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;AgeRating&gt;12&lt;/AgeRating&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;MarketData xml:lang=""en-gb""&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Name&gt;App with several in-app products&lt;/Name&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Description&gt;Sample app for demonstrating an expiring in-app product and a consumable in-app product&lt;/Description&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Price&gt;0.00&lt;/Price&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;CurrencySymbol&gt;£&lt;/CurrencySymbol&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/MarketData&gt;<br>&nbsp;&nbsp;&nbsp; &lt;/App&gt;<br>&nbsp;&nbsp;&nbsp; &lt;Product ProductId=""MORE_CASH_1000"" LicenseDuration=""0"" ProductType=""Consumable""&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;MarketData xml:lang=""en-gb""&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Name&gt;Consumable Item&lt;/Name&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Price&gt;0.99&lt;/Price&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;CurrencySymbol&gt;£&lt;/CurrencySymbol&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/MarketData&gt; <br>&nbsp;&nbsp;&nbsp; &lt;/Product&gt;<br>&nbsp; &lt;/ListingInformation&gt;<br>&nbsp; &lt;LicenseInformation&gt;<br>&nbsp;&nbsp;&nbsp; &lt;App&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;IsActive&gt;true&lt;/IsActive&gt;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;IsTrial&gt;false&lt;/IsTrial&gt;<br>&nbsp;&nbsp;&nbsp; &lt;/App&gt;<br>&nbsp; &lt;/LicenseInformation&gt;<br>&nbsp; &lt;!--<br>&nbsp; &lt;ConsumableInformation&gt;<br>&nbsp;&nbsp;&nbsp; &lt;Product ProductId=""MORE_CASH_1000"" TransactionId=""00000001-0000-0000-0000-000000000000"" Status=""Active"" /&gt;<br>&nbsp; &lt;/ConsumableInformation&gt;<br>&nbsp; --&gt;<br>&lt;/CurrentApp&gt;</code></p> <p><br>Druhý způsob je vytvoření beta aplikace. Vše se implementuje stejně, jako později do normálního vydání s tím rozdílem, že můžeme kontrolovat, kdo má do beta aplikace přístup. Nejlepší způsob je v betě vytvořit produkt, který je zadarmo a zkusit ho “nakoupit“ sám.  <p>V příštím díle se podíváme na konkrétní implementaci a problematiku durable &amp; consumable produktů a vysvětlíme si řešení free / pro &amp; ads / ads free verzemi aplikace.  <p>Toť pro dnešní díl vše a jako vždy přeji hodně štěstí a málo bugů! :)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8493, 3, CAST(N'2016-03-30T21:31:55.087' AS DateTime), N'Visual Studio 2015 Update 2 je venku', N'<p>Microsoft dnes na konferenci <a href=""www.buildwindows.com"">//build/</a> uvolnil <strong>Visual Studio 2015 Update 2</strong>. Instalátor můžete <a href=""http://go.microsoft.com/fwlink/?LinkId=691129""><strong>stáhnout zde</strong></a>.</p> <p>Co v tomto updatu přibylo nového?</p> <p>&nbsp;</p> <h2>Novinky a vylepšení</h2> <p>Nástroje <strong>Visual Studio Tools for Apache Cordova</strong> nyní podporují Cordovu ve verzi 6.</p> <p><strong>Universal Windows App</strong> nyní umožňují vybrat verzi SDK, proti které chcete targetovat, dále byl například vylepšen nástroj pro publikování na Windows Store.</p> <p><strong>C++</strong> vývojáři se mohou těšit na některé funkce ze standardů C++ 11 a C++ 14, například Variable Templates, vylepšení constexpr, coroutines a další. Bylo také opraveno mnoho bugů samotného kompilátoru, zrychleno napovídání v IDE, pár novinek je i v knihovnách pro univerzální aplikace a v nástrojích pro vývoj pro Android a iOS.</p> <p>&nbsp;</p> <h2>IDE</h2> <p>Po označení výrazu přibyla nová možnost <strong>Execute In Interactive</strong>, která vybraný kus kódu vyhodnotí v <strong>Interactive Window</strong>. </p> <p>Pokud vám v souboru chyběl <strong>using</strong>, stačilo napsat přesný název třídy a zmáčknout Ctrl-tečka, aby VS doplnění usingu nabídnulo. Nyní to funguje, i když název nenapíšete úplně přesně.</p> <p>Přibylo také několik nových code fixes, např. <em>if (handler != null) handler(this, e)</em> to umožňuje změnit na <em>handler.Invoke(this, e)</em>.</p> <p>Visual Studio nyní, stejně jako Visual Studio Code, podporuje <strong>TextMate</strong> šablony pro zvýrazňování syntaxe v různých formátech souborů, přidání podpory pro nový typ souboru do VS je tedy opět o něco jednodušší.</p> <p>Dále byla přidána možnost nastavit doplňkům, aby se samy updatovaly.</p> <p>Změn doznal i nový status bar, který usnadňuje práci se source control systémy – kromě toho, že ukazuje název aktuální větve, jsou zde i tlačítka, která umí udělat sync.</p> <p><img alt=""Version Control - Unpublished Commits example"" src=""https://i3-vso.sec.s-msft.com/dynimg/IC850208.png""></p> <p><font size=""1"">Převzato z </font><a title=""https://www.visualstudio.com/en-us/news/vs2015-update2-vs.aspx"" href=""https://www.visualstudio.com/en-us/news/vs2015-update2-vs.aspx""><font size=""1"">https://www.visualstudio.com/en-us/news/vs2015-update2-vs.aspx</font></a></p> <p>&nbsp;</p> <p>Nuget přidal také několik funkcí, mimo jiné konečně umí zobrazit balíčky <strong>ze všech feedů najednou</strong>. Kromě toho bylo UI celkově zoptimalizováno, což jsem zaznamenal okamžitě – nyní se tolik nezakousává a subjektivně mi přijde rychlejší.</p> <p><strong>TypeScript</strong> je nyní ve verzi <strong>1.8</strong>, která obsahuje také pár novinek – nejzajímavější z nich jsou <em>string literal types, this-based type guards a vylepšení union typů</em>. Kompilátor nyní umí detekovat unreachable code a formát JSX konečně umí zvýrazňování syntaxe.</p> <p>Změn doznal i <strong>Git</strong> klient ve Visual Studiu, umí pár funkcí navíc (např. cherry picking a staging, lepší filtrování v historii, podpora Large File Storage).</p> <p>Pokud vyvíjíte aplikaci v XAMLu, při spuštění aplikace se nahoře zobrazí maličký toolbar, který umožňuje spustit <strong>Live Visual Tree</strong> a další nástroje.</p> <p>A závěrem v nové verzi VS Microsoft opravil spoustu bugů, trochu zrychlil některé funkce (a není jich málo – jen v release notes mají vypsáno asi 80 věcí, kterých se to týká).</p> <p>V neposlední řadě bylo uvolněno <a href=""https://azure.microsoft.com/en-us/downloads/""><strong>Azure SDK 2.9</strong></a>.</p> <p>Kompletní seznam novinek najdete v <a href=""https://www.visualstudio.com/en-us/news/vs2015-update2-vs.aspx"">Release Notes</a>.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8494, 3, CAST(N'2016-04-19T23:53:53.487' AS DateTime), N'Náš rok s DotVVM', N'<h3>Bylo nebylo…</h3> <p><a href=""https://www.dotvvm.com""><img title=""tree"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; float: left; padding-top: 0px; padding-left: 0px; margin: 0px 20px 20px 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""tree"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Mj-rok-s-DotVVM_13755/tree_3.png"" width=""131"" align=""left"" height=""161""></a>Začalo to na <strong>MVP Summit</strong>u v listopadu 2014, kde začalo být tak nějak jasné, že Microsoft dále nebude pokračovat ve vývoji <strong>ASP.NET Web Forms</strong> a že ta technologie časem umře. Tou dobou jsme si již hráli s <strong>Knockout JS </strong>a v praxi si na několika projektech ověřili, že je to pro naše větší projekty nepraktické. Ne kvůli Knockoutu samotnému, úplně stejné by to bylo s <strong>Angular</strong>em nebo <strong>React</strong>em. Ale proto, že to jako celek smrdělo.</p> <p>Potíž byla v tom, že používat javascriptový klientský framework znamená naučit se 50 nových knihoven a toolů, což je pro ne úplně zkušené vývojáře stopka. Dále to znamená, že si musíme napsat spoustu infrastruktury sami, protože na to neexistuje vhodná knihovna, anebo jich existuje 20 a researchem, která se vám bude nejlíp hodit, zaberete víc času, než když si to napíšete sami. V neposlední řadě to znamená psát značné množství javascriptového kódu, který dohromady nic nedělá, jen vezme data odněkud (např. z REST API) a nacpe je někam (do view) anebo naopak. </p> <p>Na pár projektech jsme to zkusili a přitom jsme museli řešit desítky různých drobných technikálií, jako třeba nějaký unifikovaný mechanismus pro sortování, řazení a filtrování, abychom nemuseli v každé Web API pro tohle metodě přidávat nějaký parametr atd. OData jsou omezená a pro naše účely nevyhovovala, a tak jsme dělali vlastní řešení, což se nám moc nelíbilo. </p> <p>Nebo jak rozumně provádět validaci, když část validační logiky nelze udělat na klientovi a musí se ověřit na serveru, ale zároveň si nějak na klienta potřebujete přenést informace o tom, která políčka selhala, a obarvit je úplně stejně, jako kdyby validace selhala na klientovi. Jde to, ale musíte si to napsat sami. </p> <p>Nebo třeba hloupé zformátování data na klientovi tak, aby vypadalo stejně jako v PDF, které se generuje na serveru… Existují na to spousty knihoven, ale než je všechny projdete, vyzkoušíte, zjistíte, jestli se nehádají mezi sebou, jestli opravdu dělají to, co píší na jejich webu, a jestli nejsou zabugované a umí správně formáty, které potřebujete, tak to máte sami napsané třikrát. Kapitolou samu pro sebe je konverze lokálního času na UTC a obecně objekt <em>Date </em>v javascriptu, to je teprve skutečné peklo.</p> <p>Pro typ aplikací, které píšeme, se zkrátka žádná kombinace javascriptových nástrojů a MVC nebo Web API neosvědčila – ne, že by to v tom nešlo napsat, ale bylo to pracné a spoustu věcí jsme si museli psát sami. Logicky jsem se tedy začal poohlížet po nějakém .NETovém stacku, který by řešil naše problémy. Chtěl jsem něco, co by mi ideálně javascriptovou část vygenerovalo, protože zbavit se .NETu na serveru ve prospěch třeba Node.js by bylo mnohem pracnější, náročnější na naučení, a… prostě nemám rád Javascript, protože je to debilně navržený jazyk, kde na každé uprdnutí potřebujete nějakou knihovnu. </p> <p>Začali jsme s jakousi knihovnou, která uměla C# třídy překládat na typescriptové interfaces, pak jsme si napsali vlastní generátor Knockoutových viewmodelů, protože jsme potřebovali víc, než uměla ta knihovna, ale pořád to nebylo ono. A pak mě napadlo vzít <strong>ASP.NET Web Forms</strong>, ale místo plných HTTP POSTů použít AJAX a místo kryptického skrytého pole __VIEWSTATE přenášet jenom serializovaný JSON viewmodel z Knockoutu (resp. jen tu jeho část, která je potřeba). Celé to trochu zmodernizovat, postavit to nad MVVM (protože Knockout je taky MVVM) a vyčistit od zbytečností. Doplnit věci, které mi tam chyběly (podpora IoC/DI, data-binding na celé stránce a ne jen tam, kde se volá DataBind, nebo třeba podpora Single Page aplikací), hlavně aby v tom šly rozumně psát komponenty, a nápad byl na světě.</p> <p>&nbsp;</p> <p>&nbsp;</p> <h3>Da path</h3> <p>Dnes je to zhruba rok, co jsme na <strong>DotVVM </strong>začali pořádně pracovat. Do té doby jsem měl jen prototyp zpatlaný během týdne, kdy jsem se nudil na nějaké konferenci. Zapojil jsem do toho několik lidí z mé firmy <a href=""http://www.riganti.cz"" target=""_blank""><strong>RIGANTI</strong></a>, začali jsme pracovat na frameworku a implementovat do něj funkce, které byly potřeba, a samozřejmě to testovat. </p> <p>Mezitím se <strong>DotVVM </strong>začalo používat na projektech, které vyvíjíme pro naše zákazníky – největší, na kterém nám DotVVM běží, je už pořádně velký, strávilo se na něm už asi 6 tisíc hodin a nebýt DotVVM, bylo by to mnohem víc, a to i přes to, že jsme hlavně ze začátku podstatnou část dne místo vývoje aplikace samotné hledali, reprodukovali a opravovali bugy ve frameworku a dopisovali chybějící testy. Celkem u nás ve firmě běží v DotVVM už asi 10 aplikací, mimo jiné web <a href=""https://www.dotnetcollege.cz"" target=""_blank""><strong>dotNETcollege</strong></a>, a pracujeme na dalších a velmi zajímavých. </p> <p>Začali jsme také implementovat <strong><a href=""https://www.dotvvm.com/landing/bootstrap-for-dotvvm"" target=""_blank"">DotVVM komponenty pro Bootstrap</a></strong> – máme kompletně owrapované všechny widgety z Bootstrapu 3. Současně jsme implementovali i wrappery nad Kendo UI, nicméně ty jsme museli pozastavit, jednak kvůli nedostatku času, a jednak kvůli tomu, že se ukázalo, že Kendo je velmi zabugované a že kdybychom si to napsali sami, bylo by to jednodušší – ten jejich grid je hydra, kterou jsme s velkými obtížemi ohnuli pro použití v DotVVM, nicméně věci jako inline editace, která by se integrovala s naší validací, narazily na různé podivnosti a dost značná omezení Kendo Gridu. Jednodušší komponenty napojit šly a pořád je v kurzu možnost, že bychom udělali wrappery alespoň nad Kendo Core, které je open source a zdarma. </p> <p>Největší challenge byla <strong><a href=""https://www.dotvvm.com/landing/dotvvm-for-visual-studio-extension"" target=""_blank"">extension pro Visual Studio</a></strong>. Většina magie v DotVVM se odehrává v našem vlastním formátu DOTHTML, což je klasický HTML soubor, který může obsahovat direktivy (několik řádků na začátku souboru), serverové komponenty (např. <em>&lt;dot:TextBox … /&gt;</em>) a data-binding (<em>&lt;a href=”{value: Url}”&gt;…&lt;/a&gt;</em>). </p> <p>HTML editor ve Visual Studiu je rozšiřitelný a není tak těžké doplnit podporu pro vlastní elementy nebo jejich atributy. Dokonce tak nějak v Microsoftu počítali i s věcmi, které potřebujeme my, nicméně potíž je v tom, že dokumentace k tomu neexistuje, takže jsme strávili stovky hodin v Reflectoru a zkoumali, jak celý HTML editor funguje. Spoustu věcí jsme také opsali z <a href=""https://github.com/madskristensen/WebEssentials2015"" target=""_blank""><strong>Web Essentials</strong></a>, které jsou open source. Potkali jsme se s řadou situací, které rozhodně řešily pouze jednotky lidí na světě, a bylo to místy poučné, místy vtipné, místy šílené a místy úplně zvyjebané (snažil jsem se najít slušnější slovo, ale pro daný účel žádné takové neexistuje). Velký dík a můj neskonalý obdiv má naše Rigantí Black Magic Department, jehož členy jsou Standa Lukeš a Milan Mikuš.</p> <p>Vzhledem k tomu, že extension je hodně složitá, museli jsme si napsat i <a href=""https://www.dotvvm.com/blog/4/UI-Testing-the-Visual-Studio-Extension"" target=""_blank""><strong>vlastní framework pro její testování</strong></a><strong> </strong>(přes HTTP jí posíláme commandy a ona je ve Visual Studiu pouští a kontroluje, co se děje). </p> <p>Abychom mohli celý framework rozumně udržovat, museli jsme napsat stovky testů (včetně Seleniových UI testů, ty jsou velmi důležité) a vím jistě, že ještě několik tisíc nám jich schází. Museli jsme si napsat také vlastní <a href=""https://github.com/riganti/test-utils"" target=""_blank""><strong>framework nad Seleniem</strong></a>, aby se nám snadněji používalo a aby umělo testy pouštět rychleji a nepouštělo novou instanci browseru pro každý test. Aktuálně <strong>DotVVM</strong> testujeme v IE 11, posledním Firefoxu a Chrome, a testy pouštíme paralelně, což trvá asi 20 minut – předtím, než jsme vymysleli fast mode, to trvalo skoro 2 hodiny, což bylo neúnosné. </p> <p>Budeme muset rozjet víc agentů a testovat různé prohlížeče, včetně mobilních, už máme zjištěno, jak Selenium používat proti BrowserStacku, ale den má jen 24 hodin a značnou část ho musíte obětovat něčemu jinému.</p> <p>&nbsp;</p> <h3>Verze 1.0</h3> <p>Světlem na konci tunelu byla konference Build, kde měl mít <em>Roman Jašek</em> krátkou prezentaci nějakého projektu. Vzhledem k tomu, že v Riganti řídí největší projekt na DotVVM postavený, logicky jsme na Buildu u stánku Microsoft Student Partners prezentovali DotVVM. </p> <p>Natiskli jsme letáčky, vyrobili trička, a dnes ráno mi shodou okolností v mailu přistála gratulace a poděkování od jednoho spokojeného zákazníka, že už to začali používat a že se jim to moc líbí. Jak jsem předpokládal, ve tři ráno jsme udělali release na Nuget, nasadili web, zaklapli notebook a odjeli na letiště – stihlo se to na poslední chvíli.</p> <p>Aktuální verze je <strong>1.0</strong> <strong>BETA</strong>, od jejího vydání jsme objevili ještě několik bugů, které opravujeme a zanedlouho vydáme <strong>RC</strong>. Kromě toho jsme do prodeje vypustili <strong><a href=""https://www.dotvvm.com/landing/dotvvm-for-visual-studio-extension"" target=""_blank"">extension pro Visual Studio</a>&nbsp;</strong>a <a href=""https://www.dotvvm.com/landing/bootstrap-for-dotvvm"" target=""_blank""><strong>komponenty pro Bootstrap</strong></a>. </p> <p>Připravujeme free verzi naší VS extension, pokud si s DotVVM chcete pohrát, musíte si aktuálně stáhnout <em>30denní trial verzi</em>. </p> <p>&nbsp;</p> <h3>Zajímá vás to? Chcete se zapojit?</h3> <p>Naprogramovat to bylo snadné. Teď přijde teprve ta pravá fuška, protože o tom budu muset přednášet, psát články, nahrávat videa, vysvětlovat, k čemu to je, v čem to pomůže, že to může ušetřit peníze při vývoji aplikací atd. </p> <p>V české republice budeme v nejbližší době dělat 2 přednášky a hackathon. Pokud vás DotVVM zajímá, přijďte se podívat.</p> <ul> <li><strong>9. května 2016: Praha – <a href=""https://www.dotnetcollege.cz/konference/3/DotVVM-1-0-Moderni-webove-aplikace-bez-Javascriptu-"" target=""_blank"">Přednáška DotVVM 1.0: Moderní webové aplikace bez Javascriptu</a></strong>  <li><strong>10. května 2016: Brno – <a href=""http://www.wug.cz"">Přednáška DotVVM 1.0: Moderní webové aplikace bez Javascriptu</a></strong>  <li><strong>12. května 2016: <a href=""https://www.dotnetcollege.cz/konference/2/DotVVM-Hackathon"" target=""_blank"">Praha – DotVVM Hackathon</a></strong></li></ul> <p>Doporučuju přijít hlavně na ten hackathon, bude tam víc lidí z DotVVM týmu, takže si bude možné o DotVVM popovídat, budete se moci zeptat na cokoliv a pobavit se s námi o tom, co byste v DotVVM chtěli, co vám tam chybí, nebo co se vám nelíbí.</p> <p>Jinak pokud myslíte, že by vás práce na <strong>DotVVM </strong>bavila, tak vězte, že v <a href=""http://www.riganti.cz/jobs"" target=""_blank""><strong>RIGANTI neustále hledáme schopné lidi</strong></a>. Jednak na vývoj frameworku nebo čehokoliv, co s tím souvisí, a druhak na projekty, kde se <strong>DotVVM </strong>používá (což jsou už skoro všechny). </p> <p>&nbsp;</p> <h3>Co dál</h3> <p>Mám milion nápadů, co by se s DotVVM dalo dělat. Potřebujeme to naportovat na <strong>.NET Core</strong>, potřebujeme to umět zahostovat v rámci <strong>univerzální Windows aplikace</strong> a umožnit napsat v DotVVM univerzální appku. Hodilo by se rozběhnout to i pomocí <strong>Xamarin</strong>u, abychom v tom mohli psát jednoduché multiplatformní webové aplikace. Když může PhoneGap, proč ne my, a bez psaní javascriptu.</p> <p>Dál by se mi líbil nástroj, který by uměl projít DOTHTML soubor, najít v něm komponenty, do kterých se dá psát a klikat, a vygenerovat z toho helper třídu pro Selenium, kterou půjde snadno použít pro psaní UI testů. Pro stránku, kde budou dva TextBoxy s bindingy na FirstName a LastName, a tlačítko volající metodu Login, by to vygenerovalo třídu, která by se používala takto:</p><pre class=""brush: csharp"">var test = new LoginTestClass();
test.FirstNameTextBox.Type(""Tomáš"");
test.LastNameTextBox.Type(""Herceg"");
test.LoginButton.Click();</pre>
<p>Dále by se mi líbilo doplnit do naší Visual Studio extension spoustu funkcí, například možnost automatického konvertování <em>&lt;input type=”text”&gt; </em>na naše <em>&lt;dot:TextBox /&gt; </em>atd., což by pomohlo ve chvíli, kdy dostanete nakódované HTML od grafika, který DotVVM nezná. Nebo nějaký pěkný scaffolding pro GridView, více refactorovacích nástrojů, design time error checking pravidel atd. </p>
<p>&nbsp;</p>
<h3>Na dobrou noc</h3>
<p>Když jsem se nedávno podíval, kolik mě DotVVM stálo, a započítal, kolik jsem mohl vydělat, kdybych místo toho pracoval na něčem, co generuje přímý zisk, tak mi (s trochou nadsázky) vyšlo, že jsem si mohl pořídit dům nebo dvě Tesly S. Nicméně jen na našich projektech už DotVVM ušetřilo velkou část toho, co nás stálo, takže i kdyby ho nikdo kromě nás nechtěl, časem se nám ta investice vrátí. Nehledě na to, že jsme se naučili obrovské kvantum nových věcí, získali spoustu nových zkušeností, a že nás to (aspoň většinu času) bavilo a posunulo dál.</p>
<p>Nicméně, během těch 14 dní, co se DotVVM dá koupit, jsme již několik licencí prodali (dokonce většinu mimo ČR), a ohlasy jsou na to velmi pozitivní, přišlo mi už několik mailů od různých lidí, že to je přesně to, co hledali.</p>
<p>&nbsp;</p>
<p>Na tomto místě bych chtěl <strong>poděkovat všem, kteří se na DotVVM podíleli</strong>, ať už přímo tím, že pro něj něco vyvíjeli, anebo i tím, že pracovali na jiných projektech a umožnili tím, aby se na vývoj mohli soustředit ostatní.</p>
<p>A samozřejmě i těm mimo naši firmu, kteří DotVVM již začali používat a posílali nám celou dobu cenný feedback. Díky. Bez vás by se to nepovedlo.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8495, 4, CAST(N'2016-05-05T10:20:06.223' AS DateTime), N'ed For Theme Detection (fb785f2f-e030-4b78-b694-de98202bf0e5 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)', N'<p>This is a temporary post that was not deleted. Please delete this manually. (4f066ee0-8ff4-4aae-ad5d-937bd55fb472 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8496, 4, CAST(N'2016-05-05T14:09:52.157' AS DateTime), N'Proč nemám rád Entity Framework', N'<p>Ačkoliv <strong>Entity Framework</strong> většina z vás zná a používá, dovolím si krátký úvod. Je jednou z vlajkových technologií pro vývoj v .NET Frameworku. Jedná se o ORM (mapper objektů na relační data) na steroidech, který dovoluje přistupovat, dotazovat a upravovat data z tabulek databází pomocí silně typových tříd (entity) a LINQ syntaxe (query). V ideálním případě vás plně odstíní od používání jazyka SQL a dovoluje pracovat s daty, jako kdyby se jednalo o kolekce přímo v .NET prostředí.</p> <p>Mezi hlavní výhody řadím, krom právě silné typovosti a LINQ dotazů, sledování změn, lazy loading, migrace struktury databáze a její kontrolu a neposlední řadě projekci (lze provést projekční dotaz načítající jen požadované sloupce a to i napříč více “najoinovanými” tabulkami skrze cizí klíče).</p> <h2>Úskalí použití Entity Frameworku</h2> <p>Každá technologie má scénáře kdy se ji vyplatí používat a kdy naopak ne. Osobně EF velmi rád využívám a šetří mi mnoho času. Jsou ale projekty, kde ho buď použít nechci vůbec nebo jen ve velmi omezené míře. Berte proto následující seznam jako body k zamyšlení, zda je opravdu EF pro váš další projekt vhodný.</p> <h3></h3> <h3>Rychlost</h3> <p>Rychlost (respektive pomalost) je jeden z častých argumentů právě proti EF. Sám jej nevnímám jako zásadní problém, je však dobré s tímto aspektem počítat.</p> <p>První věc, která často vývojáře zarazí je problém rychlosti spuštění. Právě při prvním dotazu se EF může až na několik vteřin “zamyslet” – připravuje vnitřní struktury a kontroluje řadu věcí. Další dotazy již žádné takové zpoždění nemají. Pro naprostou většinu projektů je lehké prodloužení startu aplikace zanedbatelné.</p> <p>Druhý a zásadnější problém je rychlost dotazů a aktualizace dat v databázi. Zpomalení zde může být znatelné, pokud načítáte a aktualizujete větší počty záznamů nebo větší objektové grafy. Samo o sobě je toto zpomalení pochopitelné, protože EF dělá řadu věcí, které prostě celý proces zpomalí. </p> <p>Je dobré zmínit, že EF ve výchozím stavu jede v “plné palbě” a je možné ho zrychlit například vypnutím sledování změn a lazy loadingu (proxy tříd), pokud nehodláte načtená data modifikovat.</p> <p>Osobně nevidím rychlost EF jako příliš velký problém. Je to prostě daň za pohodlnější a rychlejší vývoj (v některých případech), se kterou je potřeba počítat. Navíc lze často využívat hybridní přístup, kde EF používám na věci, kde výkonnostně stačí a na zbytek použiji přímo uložené procedury a ADO.NET nebo nějaký tenký mapper. Může jít například o hromadnou modifikaci mnoha řádků.</p> <p>Jako vždy u rychlosti platí, že nejlepší je provést test rychlosti a pak teprve vynášet soudy a řešit další optimalizace.</p> <h3>Generované SQL</h3> <p>Pro databázové architekty může být nepříjemné, že dopředu neznají jaké dotazy se budou používat a jak budou sestavené – generuje je přímo EF a může se během vývoje rapidně měnit bez vědomí vývojáře. A s největší pravděpodobní budou dotazy velmi ošklivé. To je ovšem u systému jako EF pochopitelné. Nicméně jen hledání chyb v takovém rozsáhlém SQL není úplné příjemné.</p> <h3>Volba architektury</h3> <p>EF vždy pracuje v rámci Unit Of Work patternu (DataContext objekt), který reprezentuje logické připojení do databáze, sledování změn, které se propíší do databáze a je vstupním bodem pro sestavení dotazů. Změny na všech upravených objektech v rámci kontextu pak uložíte zavoláním SaveChanges metody. Poznámka: Vytvořený DataContext neznamená, že je automaticky otevřené spojení, to se otevírá a zavírá až v případě potřeby. </p> <p>Při práce s EF máte v zásadě dvě možnosti. Buď použijete EF jen v rámci datové vrstvy jako pomocníka pro přístup k datům a vyšší vrstvy o něm již nebudou vědět – budou používat repositáře, commandy a různé pohledy. Druhá, rychlejší a jednodušší cesta je dát k dispozici DataContext všem, kdo jej potřebuje a budeme jej považovat přímo za datovou vrstvu. První postup se hodí na větší aplikace a naopak druhý je obvykle dostačující u jednodušších projektů. Samozřejmě rozhodující kritérium není jen velikost aplikace ale převážně požadavky na architekturu.</p> <p>V dalších částech popisuji nevýhody jednotlivých postupů.</p> <h3>Nevýhody použití EF jako DL</h3> <p>Pokud se rozhodnete využívat přímo Entity Framework jako datovou vrstvu, můžete postupem času narazit na různé překážky.</p> <p>Z architektonického pohledu je tu problém prakticky nulové abstrakce a testovatelnosti. Používáte přímo DataContext a jeho DbSet pro přístup ke všem tabulkám. Tyto objekty jde jen velmi těžko nahradit například za třídy pro přístup k jinému uložišti a prorostou nemalou částí aplikace. Zároveň může kterákoliv část aplikace používat přímo tabulky ze kterékoliv části databáze.</p> <p>To má za následek často problém duplikace a rozrůstání databázových query napříč částmi aplikaci. Příkladem je načítání dejme tomu kategorií článků –&nbsp; 30 různých míst v aplikaci je potřebuje načítat a vy se rozhodnete přidat sloupec “IsDeleted” pro označení smazané kategorie, která se nemá zobrazovat. Vy tak následně musíte provést refactoring všech částí aplikace, které do tabulky kategorie přistupují. Čím bude podmínek více, tím komplexnější musíte rozšiřovat dotazy a tím větší pravděpodobnost chyby nastává. Navíc v praxi mohou být podmínky podstatně složitější a mohou prorůstat přes řadu tabulek.</p> <h3>Prorůstání IQueryable</h3> <p>Nepříjemným problém může být prorůstání <strong>IQueryable&lt;Entity&gt;</strong> napříč projektem. IQueryable je rozhraní popisující zdroj dat, nad kterým sestavujeme strom podmínek, řazení a projekcí a až při dotázání na data (foreach, ToArray apod.) se dotaz uskuteční. Dostat se k takové query můžete přímo přes DataContext přístupem k entitě nebo i přes lazy loading relací u entit (například <em>article.Categories</em>).</p> <p>Teoreticky by nám mělo být jedno, zda je zdrojem dat přímo databáze nebo pole v paměti aplikace. Ve skutečnosti ale může být výsledek různý. Například u jednoduchého dotazu <em>Where(u =&gt; u.Email == email)</em> provedený proti poli v paměti bude výsledek rozlišovat velikost písmen (porovnání v .NET). Pokud jej provedeme proti EF tabulce, velikost písmen rozlišovat nebude (porovnání v SQL; záleží na nastavení sloupce v databázi). Reálný problém je pak situace, kdy část aplikace počítá s použitím přímo databázového query – často i nevědomě. Následně někdo provede refactoring a místo EF dotazu začne předávat běžné pole; na venek se vše tváří stejně, ale porovnávání začne rozlišovat velikost písmen. Takové problémy se hledají velmi špatně a prorostou celou aplikací.</p> <p>Osobně mám nejradši, pokud IQueryable neopustí datovou vrstvu. Pokud potřebuji provádět filtrování, vytvořím si objekt popisující filtry a ten aplikuji na IQueryable přímo v datové vrstvě.</p> <h3>Lazy Loading</h3> <p>Lazy loading zajišťuje načítání relačních vazeb až v době, kdy k nim přistoupíme. Tato na jednu stranu užitečná funkce může způsobit i problémy. </p> <p>Dejme tomu, že v databázové vrstvě přestanete načítat k entitě její relační vazbu (například <em>article.Tags</em>) a na prezentační vrstvě každý řádek zobrazení článku ukazuje i tyto tagy. Proto každý řádek provede lazy loading vlastnosti <em>Tags</em>. Z jednoho dotazu jich najednou bude v lepším případě třeba 50.</p> <p>Toto je však obecně problém týkající se lazy loadingu. Výhodou je, že lze i v EF vypnout a vy tak snadněji zjistíte, že jste zapomněli načítat něco, co aplikace potřebuje.</p> <h3>Problém izolace repozitářů</h3> <p>Pokud se rozhodnete, že budete používat EF jen jako pomocníka v datové vrstvě, pravděpodobně vytvoříte pro přístup k datům repositáře pro jednotlivé entity – například repositář pro entity zákazníků a repositář pro entity faktur. Z pohledu repositáře je entita zákazníka samostatná – ukládáte ji samostatně bez ohledu na ostatní entity. </p> <p>Pokud však používáte EF a v jedné části aplikace upravíte tabulku zákazníka v druhé části tabulku faktur, nemáte možnost, jak uložit jen entity například faktury. Zavolání SaveChanges uloží všechny změny v rámci DataContext.</p> <p>Nejčastější řešení je volat SaveChanges ve všech write metodách repositářů aby nebylo možné opustit metodu bez provedení uložení. Problém nastává, pokud vývojář získá entity z repositáře, upraví je a nakonec se je rozhodne neuložit (například neprošla validace). Pokud nyní nic dalšího neprovede, entita se neuloží. Pokud však zavolá i naprosto nesouvisející repositář pro uložení úplně jiné entity, pak se uloží i rozpracovaná entita díky systému sledování změn.</p> <p>I zde je možné tento problém vyřešit vypnutím automatických sledování změn a změny oznamovat manuálně v repositářích. </p> <h3>Mazání z vazebních tabulek</h3> <p>Entity se kterými pracujeme v rámci repositářů označujeme jako tzv. agreggate roots. Tedy minimální graf objektů, který načítáme/ukládáme. Například entita zákazníka po načtení z repositáře obsahuje i svoje adresy a zároveň při ukládání vždy ukládáme celého zákazníka i adresami.</p> <p>Pokud v EF odstraníte entitu z kolekce relace (například <em>customer.Addresses.Remove(address)</em>), neprovede se její smazání – pouze dojde k rozvázání vazby (zde například mezi adresou a zákazníkem). Pro smazání adresy je nutné zavolat <em>DeleteObject</em> přímo na DataContext objektu. To bohužel znamená, že pokud chceme adresu smazat, nestačí ji jen smazat z entity zákazníka a zavolat SaveChanges ale musíme ale navíc přistoupit přímo k DataContext (od kterého by nás měl repositář odstínit).</p> <p>Řešení této situace je trochu složitější. Je možné nahradit v entitách kolekce při načítání za vlastní, které sami sledují smazané položky a ty interně mažou i přímo na DataContextu.</p> <h2>Závěr</h2> <p>Vhodné postupy a problémy se obvykle liší projekt od projektu. Berte prosím mé názory jako doporučení k zamyšlení a ne jako dogmatický návod co je dobře a co je špatně. Bohu rád, pokud se se mnou podělíte o zkušenosti v diskusi.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8498, 3, CAST(N'2016-07-25T21:26:40.097' AS DateTime), N'CORESTART 2016: Přijďte se podívat na konferenci o .NET Core a Windows Server 2016', N'<p>Microsoft nedávno uvolnil ostrou verzi nové generace vývojářské platformy <strong>.NET Core</strong>, a již za pár měsíců se chystá vydat další verzi svého klíčového produktu <strong>Windows Server 2016</strong>. <p><strong>Konference CORESTART</strong>, to jsou tři dny plné novinek pro vývojáře a IT odborníky. Přijďte se podívat a dozvědět se něco nového o technologiích, které jsou aktuálně v kurzu. <p>Konference se koná <strong>17. - 19. srpna 2016</strong> v sídle společnosti <strong>Microsoft</strong>. <p>&nbsp; <p>Registrujte se <strong><a href=""https://www.corestart.cz"">na webu konference CORESTART 2016</a></strong>!</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8499, 3, CAST(N'2016-09-26T18:48:19.007' AS DateTime), N'MSTest na .NET Core - Jak na to?', N'<p>Na <strong><a href=""https://www.dotvvm.com"">DotVVM</a> </strong>právě dokončujeme migraci na <strong>.NET Core </strong>a důležitou součástí jsou testy. Potřebujeme testovat různé kombinace, protože potřebujeme podporovat velký .NET Framework, Mono i .NET Core, a zároveň je kromě nového ASP.NET hlavní platformou OWIN. </p> <p>A samozřejmě to potřebujeme testovat jak na Windows, tak na Linuxu a Mac OS. Máme testy na framework samotný (unit a integrační testy), a pak máme mnoho UI testů v Seleniu, které otevřou prohlížeč, fyzicky klikají do stránky a kontrolují, že se děje to, co by se dít mělo. </p> <p>&nbsp;</p> <p>Z historických důvodů jsme používali <strong>MSTest</strong>, což sklízelo u některých lidí udivené reakce, že jako proč ne <strong>xUnit </strong>a kdesi cosi. V reálu mě mnohem víc pálí to, že máme testů stovky, přičemž bychom jich měli mít tisíce.</p> <p>Nicméně, Microsoft po letech nečinnosti s <strong>MSTest</strong>em<strong> </strong>konečně začal něco dělat, a díky tomu je možné napsat testy tak, aby běžely jak na plném .NETu, tak i na .NET Core.</p> <p>&nbsp;</p> <p>Předpokládejme, že testovaná aplikace je <strong>Class Library</strong>, která podporuje platformy <strong>net461 </strong>(plný .NET 4.6.1) a <strong>netcoreapp1.0</strong> (tedy .NET Core), a chceme k ní udělat testy.</p> <p>Je potřeba udělat následující kroky:</p> <p>&nbsp;</p> <p>1. Přidáme do solution nový projekt typu <strong>Console Application (.NET Core)</strong>.</p> <p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/f425d45d5e4d_102BA/image_2.png""><img title=""Přid&aacute;n&iacute; konzolov&eacute; aplikace pro testy"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""Přid&aacute;n&iacute; konzolov&eacute; aplikace pro testy"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/f425d45d5e4d_102BA/image_thumb.png"" width=""562"" height=""390""></a></p> <p>&nbsp;</p> <p>2. Smažeme z ní <strong>Program.cs </strong>a z <strong>project.json </strong>vymažeme <strong>“emitEntryPoint”: “true”</strong>.<br></p> <p>3. Přidáme následující Nuget balíčky (v <strong>project.json</strong> do sekce <strong>dependencies</strong>):</p> <p><strong>- ""MSTest.TestFramework"": ""1.0.1-preview""</strong> (samotný testovací framework)</p> <p><strong>- ""dotnet-test-mstest"": ""1.1.1-preview"" </strong>(test runner, který řekne VS, jak má testy hledat, a umožní je spustit z command line)</p> <p>&nbsp;</p> <p>4. Dále je samozřejmě nutné nareferencovat testovaný projekt, takže do sekce <strong>dependencies</strong> přidáme ještě toto:</p><pre><code class=""language-JS"">""MsTestDemo.App"": {<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ""target"": ""project""<br>&nbsp;&nbsp;&nbsp; }</code></pre>
<p>&nbsp;</p>
<p>5. Dále je třeba přidat pod <strong>version</strong> ještě tuto klauzuli:</p><pre><code class=""language-JS"">""testRunner"": ""mstest""</code></pre>
<p>&nbsp;</p>
<p>Nyní stačí napsat samotné testy a projekt by měl jít normálně zkompilovat. </p>
<p>Testy by měly jít spustit jak z okna <strong>Test Explorer</strong>, tak z příkazového řádku pomocí volání <strong>dotnet test</strong>. Spustí se na všech platformách, které jsou specifikované v <strong>project.json</strong>.</p>
<p>&nbsp;</p>
<p>Pro demonstraci jsem připravil 
<span id=""scid:6E335E69-603A-4F1E-AA4B-726D584133B4:571551b3-a098-4e34-bc10-e0a8be8b8aa2"" class=""wlWriterSmartContent"" style=""float: none; padding-bottom: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline-block; padding-right: 0px""><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/f425d45d5e4d_102BA/ce71eb61-2e86-4067-b52d-bf965a96cfe1_mstest_dotnetcore.zip""><strong>ukázkový projekt</strong></a></span>.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8500, 4, CAST(N'2016-10-20T08:13:55.177' AS DateTime), N'ed For Theme Detection (1da3e5e6-5cd2-4a2d-87bd-f8f725b30364 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)', N'<p>This is a temporary post that was not deleted. Please delete this manually. (11522893-a9f4-4db1-b6f2-fb72df970a86 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8501, 7, CAST(N'2016-10-20T08:31:26.427' AS DateTime), N'Školení IT – efektivní způsob vzdělávání [reklamní sdělení]', N'<p>Někdo nedá dopustit na nejrůznější tutoriály, jiný dává přednost příručkám a návodům. Ovšem většina lidí jistě bude souhlasit s tím, že z hlediska efektivity a časové náročnosti je zdaleka nejlepší volbou školení. Ale vezměme to pěkně popořádku. Jakými způsoby můžete prohloubit vaše kompetence v oblasti informačních technologií?  <h3>Jak zvýšit odbornost</h3> <p>Zde se nabízí množství způsobů, ty je však třeba volit s ohledem na vaši vstupní úroveň. Pro začátečníky jsou pravděpodobně nejvhodnějším způsobem školení a kurzy. Díky zpětné vazbě a možnosti si veškeré nabyté znalosti okamžitě vyzkoušet je tento způsob mnohem efektivnější, zejména v porovnání se studováním nejrůznějších příruček, které jsou často příliš zastaralé.  <p>Ovšem i profesionálům mají školení co nabídnout. Kurz vedený erudovaným odborníkem vám ušetří spoustu času a starostí, neboť lektor se s vámi rád podělí o své zkušenosti a nejrůznější tipy, které vám značně zjednoduší vaši práci. Užitečné jsou i nejrůznější přednášky a prezentace, ale zde už většina lidí postrádá možnost interakce. Na internetu se sice dá najít téměř všechno, ale informace mají často pochybný charakter. Vzpomeňte si, kolik času jste naposledy ztratili zkoušením naprosto „zaručeného“ postupu?  <p>I pokud se nevěnujete primárně informačním technologiím, má pro vás další vzdělávání v této oblasti své jasné opodstatnění. Existuje totiž přehršel užitečných nástrojů. Nejedná se jen o běžné používané systémy a programy. Specializované programy vám mohou pomoci zajistit plynulý chod vaší firmy.  <h4>VMware</h4> <p>Díky využití řešení VMware dochází ke zjednodušení prostředí, což poskytovatelům dovoluje poskytovat flexibilnější služby. Na první pohled neproniknutelné funkce se vám jistě podaří zvládnout, pomoci vám k tomu mohou workshopy a autorizované kurzy. Mimo jiné si můžete vybrat z těchto <a href=""http://www.skoleni-ict.cz/podkategorie/VMware-VMwar.aspx"">školení VMware</a>:  <ul> <li>Instalace a administrace  <li>Rozšířená správa  <li>Virtuální desktopy  <li>Zálohování a obnova</li></ul> <h4>MS SQL</h4> <p>Microsoft SQL Server představuje řešení v oblasti databází. Nabízí vysokou výkonnost, dostupnost i zabezpečení. Pomocí školení se nástroje pro správu naučíte ovládat dle vašich potřeb. Namátkou se jedná například o <a href=""http://www.skoleni-ict.cz/podkategorie/MS-SQL-MSSQ.aspx"">školení SQL server</a> zaměřené na:  <ul> <li>Základy jazyka SQL v SQL serveru  <li>Pokročilé programování  <li>Administrace  <li>Optimalizace výkonu  <li>Přechod ze starších verzí  <li>Analýza dat pro analytiky a vývojáře  <li>Zálohování a obnova databází  <li>Tvorba reportů  <li>Rozšiřování a programování funkcionalit</li></ul> <p>Využijte všech možností, které informační a komunikační technologie nabízí. Přesvědčte se o tom, že investice se vám bohatě vrátí.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8502, 7, CAST(N'2016-10-20T08:31:01.077' AS DateTime), N'ed For Theme Detection (17eb49ab-d487-4658-9a63-83df1168c3c7 - 3bfe001a-32de-4114-a6b4-4005b770f6d7)', N'<p>This is a temporary post that was not deleted. Please delete this manually. (0c0d7420-5f01-41fb-8316-2a71d46245fa - 3bfe001a-32de-4114-a6b4-4005b770f6d7)</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8503, 7, CAST(N'2016-10-20T21:24:20.960' AS DateTime), N'Školení IT – efektivní způsob vzdělávání', N'<p><em>Reklamní sdělení</em> <p>Někdo nedá dopustit na nejrůznější tutoriály, jiný dává přednost příručkám a návodům. Ovšem většina lidí jistě bude souhlasit s tím, že z hlediska efektivity a časové náročnosti je zdaleka nejlepší volbou školení. Ale vezměme to pěkně popořádku. Jakými způsoby můžete prohloubit vaše kompetence v oblasti informačních technologií? <h4>Jak zvýšit odbornost</h4> <p>Zde se nabízí množství způsobů, ty je však třeba volit s ohledem na vaši vstupní úroveň. Pro začátečníky jsou pravděpodobně nejvhodnějším způsobem školení a kurzy. Díky zpětné vazbě a možnosti si veškeré nabyté znalosti okamžitě vyzkoušet je tento způsob mnohem efektivnější, zejména v porovnání se studováním nejrůznějších příruček, které jsou často příliš zastaralé. <p>Ovšem i profesionálům mají školení co nabídnout. Kurz vedený erudovaným odborníkem vám ušetří spoustu času a starostí, neboť lektor se s vámi rád podělí o své zkušenosti a nejrůznější tipy, které vám značně zjednoduší vaši práci. Užitečné jsou i nejrůznější přednášky a prezentace, ale zde už většina lidí postrádá možnost interakce. Na internetu se sice dá najít téměř všechno, ale informace mají často pochybný charakter. Vzpomeňte si, kolik času jste naposledy ztratili zkoušením naprosto „zaručeného“ postupu? <p>I pokud se nevěnujete primárně informačním technologiím, má pro vás další vzdělávání v této oblasti své jasné opodstatnění. Existuje totiž přehršel užitečných nástrojů. Nejedná se jen o běžné používané systémy a programy. Specializované programy vám mohou pomoci zajistit plynulý chod vaší firmy. <h4>VMware</h4> <p>Díky využití řešení VMware dochází ke zjednodušení prostředí, což poskytovatelům dovoluje poskytovat flexibilnější služby. Na první pohled neproniknutelné funkce se vám jistě podaří zvládnout, pomoci vám k tomu mohou workshopy a autorizované kurzy. Mimo jiné si můžete vybrat z těchto <a href=""http://www.skoleni-ict.cz/podkategorie/VMware-VMwar.aspx"">školení VMware</a>: <p>· Instalace a administrace <p>· Rozšířená správa <p>· Virtuální desktopy <p>· Zálohování a obnova <h4>MS SQL</h4> <p>Microsoft SQL Server představuje řešení v oblasti databází. Nabízí vysokou výkonnost, dostupnost i zabezpečení. Pomocí školení se nástroje pro správu naučíte ovládat dle vašich potřeb. Namátkou se jedná například o <a href=""http://www.skoleni-ict.cz/podkategorie/MS-SQL-MSSQ.aspx"">školení SQL server</a> zaměřené na: <p>· Základy jazyka SQL v SQL serveru <p>· Pokročilé programování <p>· Administrace <p>· Optimalizace výkonu <p>· Přechod ze starších verzí <p>· Analýza dat pro analytiky a vývojáře <p>· Zálohování a obnova databází <p>· Tvorba reportů <p>· Rozšiřování a programování funkcionalit <p>Využijte všech možností, které informační a komunikační technologie nabízí. Přesvědčte se o tom, že investice se vám bohatě vrátí.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8504, 3, CAST(N'2016-11-30T13:14:14.097' AS DateTime), N'WUG Days 2016: Záznamy z přednášek', N'<p>Minulý měsíc jsem přednášel na konferenci <strong>WUG Days 2016</strong> v Brně. Dovolím si zde sdílet záznamy z mých přednášek:</p> <p>&nbsp;</p> <h3>.NET Core: Co vás čeká</h3> <p><a title=""http://www.wug.cz/zaznamy/356-WUG-Days-2016-NET-Core-Co-vas-ceka"" href=""http://www.wug.cz/zaznamy/356-WUG-Days-2016-NET-Core-Co-vas-ceka"">http://www.wug.cz/zaznamy/356-WUG-Days-2016-NET-Core-Co-vas-ceka</a></p> <p>&nbsp;</p> <h3>AutoMapper a jak se z něj nezbláznit</h3> <p><a title=""http://www.wug.cz/zaznamy/353-WUG-Days-2016-AutoMapper-a-jak-se-z-nej-nezblaznit"" href=""http://www.wug.cz/zaznamy/353-WUG-Days-2016-AutoMapper-a-jak-se-z-nej-nezblaznit"">http://www.wug.cz/zaznamy/353-WUG-Days-2016-AutoMapper-a-jak-se-z-nej-nezblaznit</a></p> <p>&nbsp;</p> <h3>DotVVM na .NET Core</h3> <p><a title=""http://www.wug.cz/zaznamy/355-WUG-Days-2016-DotVVM-na-NET-Core"" href=""http://www.wug.cz/zaznamy/355-WUG-Days-2016-DotVVM-na-NET-Core"">http://www.wug.cz/zaznamy/355-WUG-Days-2016-DotVVM-na-NET-Core</a></p> <p>&nbsp;</p> <h3>Co nevíte o Javascriptu</h3> <p><a title=""http://www.wug.cz/zaznamy/347-WUG-Days-2016-Co-nevite-o-Javascriptu"" href=""http://www.wug.cz/zaznamy/347-WUG-Days-2016-Co-nevite-o-Javascriptu"">http://www.wug.cz/zaznamy/347-WUG-Days-2016-Co-nevite-o-Javascriptu</a></p> <p>&nbsp;</p> <p>&nbsp;</p> <p>David Gešvindr průběžně zpracovává záznamy ze všech přednášek, stačí tedy sledovat stránku <a title=""http://www.wug.cz/zaznamy"" href=""http://www.wug.cz/zaznamy"">http://www.wug.cz/zaznamy</a>. </p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8507, 4, CAST(N'2016-12-22T17:55:18.973' AS DateTime), N'SQL Server: Průvodce indexy', N'<p>Víte, jaký je rozdíl mezi CLUSTERED INDEX, UNIQUE INDEX a PRIMARY KEY? Jak zvolit správný typ indexu a jak tím bude ovlivněn výkon databáze? Jakým způsobem SQL Server ukládá data do fyzických datových souborů a jak je využívá pro čtení? To jsou témata, kterým se v tomto článku budu věnovat. Považuji je ze nutnou znalost pro správný návrh a optimalizaci datových struktur v SQL Serveru.</p> <h2>Struktura datového souboru</h2> <p>SQL Server v běžné konfiguraci ukládá stav databáze do<strong> datových MDF soubor</strong>ů a log změn do <strong>LDF souborů transakčního logu</strong>. Vzhledem k tématu článku nás bude nyní zajímat jen datový soubor. Vzhledem k rozsahu témat účelově zjednoduším některá z detailů – například extends nebo LOB a overflow data.</p> <p>Datový soubor je pravidelně rozdělen po 8KB na <strong>pages</strong> (stránky). Tyto jsou jejich základní typy:</p> <ul> <li><strong>Prázdné stránky</strong></li> <li><strong>Speciální</strong> – například hlavičky datového souboru a další jinam nezařazené – zde pro nás nejsou důležité a nebudu se jim věnovat</li> <li><strong>Globální alokační mapy </strong>(GAM, SGAM, PFS) – informace o použitých / volných stránkách pro celý soubor</li> <li><strong>Alokační mapy </strong>– seskupují související stránky jednoho indexu</li> <li><strong>Datové</strong> – obsahují řádky dat</li> <li><strong>Index</strong> – obsahují strom indexu</li></ul> <p><strong>Prázdné stránky</strong> nejsou využité a SQL Server je může v případě potřeby použít, pokud bude třeba alokovat novou stránku. Pokud volné stránky dojdou, zvětší se celý datový soubor a tím se získá nový rozsah stránek připravených k použití.</p> <p><strong>Globální alokační mapy</strong> nesou právě informace, které stránky jsou volné a které využité. Vyskytují se v pevných intervalech a využívají se mimo jiné při hledání další volné stránky při zápisu nových dat. Poměr prázdných stránek uvidíte i ve vlastnostech databáze v polích <em>Size </em>(celková velikost datového souboru) a <em>Space Available</em> (velikost nevyužívaných stránek).</p> <p>Základní fyzickou strukturou pro ukládání řádků jsou <strong>indexy</strong> (dříve a interně stále označovány jako tzv. <strong>rowsets</strong>). Index má podle potřeby alokovaný určitý počet stránek (podle množství a velikosti dat) a jejich seznam udržuje právě <strong>alokační mapa</strong>. Kterou má index minimálně jednu alokační mapu. Díky alokační mapě tedy SQL Server ví, které stránky patří jakému indexu.</p> <p><strong>Datové stránky</strong> obsahují jednotlivé datové řádky v rámci indexu. Datové stránky obsahují navíc odkaz na předchozí a následující stránku, pokud se všechny řádky nevejdou na jednu stránku. Vytváří tak řetěz všech stránek daného indexu. Například pokud SQL Server čte z indexu všechna data, stačí mu načíst první stránku a pak pomocí odkazů na další pokračovat tak dlouho, než dojde na konec. </p> <p><strong>Stránky stromu indexu </strong>jsou doplněním datových stránek sloužících pro rychlejší vyhledávání. Struktura je uložena pomocí algoritmu b-tree (nikoliv <strike>binary tree</strike>) a odkazuje na <em>datové stránky</em>. Při použití těchto stránek je nutné, že uložená data v datových stránkách budou seřazeny vybraného sloupce nebo sloupců, aby bylo podle něj/nich možné vyhledávat.&nbsp; </p> <p>Může to být trochu matoucí – index musí vždy obsahovat datové stránky, ale nemusí nutně obsahovat strom indexu. Pokud máme pouze datové stránky, označujeme takový index jako <strong>HEAP</strong>. Důležité pro nás je:</p> <ul> <li><strong>index typu HEAP </strong>dovoluje pouze sekvenční procházení dat – od začátku dokonce nebo naopak, data v HEAP struktuře nejsou nijak seřazena</li> <li><strong>běžný INDEX </strong>musí být seřazen podle jednoho nebo více sloupců jeho dat; zabírá více místa a je náročnější na zápis (režie se řazením a stromem), ale ve většině scénářů je výhodnější, než <em>HEAP</em>; dovoluje totiž snadno a rychle podle seřazeného sloupce vyhledávat</li></ul> <h3>Shrnutí</h3> <p>Základní struktura na ukládání řádků je <strong>index</strong>. Ten může být buď tupé uložiště neseřazených dat – HEAP. Nebo může být index uložený se stromem indexu, seřazený podle jednoho nebo více sloupců. </p> <p>Velikost datové části indexu roste poměrově s množstvím uložených dat. Pokud se nejedná o HEAP, pak navíc velikost stromu indexu roste s velikostí dat, podle který se řadí. Tedy menší bude strom indexu seřazený podle čísla, než podle dlouhého řetězce.</p> <h2>Metadata tabulek</h2> <p>Mějme modelový příklad – databáze a v ní jsme právě vytvořili novou tabulku. SQL server uloží do datového souboru databáze informace o jméně tabulky, sloupcích, primárním klíči, indexech a obecně všem, co o ní potřebuje vědět.</p> <p>Fyzicky se tyto metadata ukládají do systémových tabulek. Přímo tyto tabulky nejsou běžným způsobem přístupné, ale můžeme do nich nahlížet skrze tzv. systémové pohledy. Příkladem je <em>sys.tables</em> a <em>sys.columns</em> obsahující zjednodušené informace o tabulkách a sloupcích. Důležité je, že o tyto systémové tabulky se server stará sám a nemusíme (a vlastně ani nemůžeme) nijak zasahovat do jejich struktury.</p> <h2>Data tabulek</h2> <p>Z pohledu optimalizací a návrhu je nutné rozlišovat mezi logickým pohledem na data a fyzickou strukturou na disku.</p> <p>Pokud například přidáte nový index, logická struktura tabulky se nezmění – stále bude mít stejné sloupce a data tabulky se nezmění. Nicméně bude ovlivněn způsob uložení na disku a způsob jakým se budou data z disku číst a zapisovat, což může mít přímý dopad na výkon.</p> <h3></h3> <h2>Typy indexů z pohledu tabulek</h2> <p>Logický pohled na data budu záměrně označovat jako <strong>tabulky</strong> – tedy běžná tabulka, tak jak ji známe. Každá taková tabulka se fyzicky na disku skládá z jednoho nebo více <strong>indexů</strong> popsaných výše.</p> <p>Při čtení z tabulky se SQL Server rozhodne, který index nebo skupina indexů se využije tak, aby bylo čtení co nejméně náročné. Stejně tak při zápisu vybírá, které indexy je nutné v rámci konkrétních změn upravit.</p> <p>Každá tabulka má:</p> <ul> <li>vždy <strong>jeden hlavní index</strong>, který je buď typu <strong>HEAP</strong> nebo běžný index označovaný jako <strong>CLUSTERED INDEX</strong> – tento hlavní index obsahuje vždy všechny řádky i sloupce tabulky</li> <li>a <strong>0 až n vedlejších indexů</strong> označovaných jako <strong>NONCLUSTERED INDEX</strong> – mohou obsahovat pouze některé sloupce a některé řádky</li></ul> <p>Strukturu hlavního indexu typu HEAP použije SQL Server automaticky v případě, kdy tabulce nenadefinujeme žádný CLUSTERED INDEX. Ve většině případů je ale výhodnější CLUSTERED INDEX definovat a vyhnout se tak typu HEAP. Navíc CLUSTERED INDEX se automaticky vytvoří při zakládání tabulky, pokud zvolíme primární klíč.</p> <h2>Nastavení indexů</h2> <p>Když nad tabulkou vytváříme indexy, můžeme měnit toto nastavení:</p> <ul> <li>Sloupec nebo sloupce, podle kterých bude seřazen – pokud je sloupců více, je důležité pořadí – pokud například seřadíte telefonní seznam první podle jména a pak teprve příjmení bude hledání jen podle příjmení náročnější, než kdyby bylo řazení první podle příjmení</li> <li>Typ CLUSTERED a NON CLUSTERED – maximálně 1 CLUSTERED a libovolné množství NON CLUSTERED indexů</li> <li>UNIQUE – nedovoluje mít duplicitní hodnoty ve vybraných sloupcích řazení; tento příznak mohou mít oba typy indexů; při zápisu dat pouze SQL Server v indexu pokusí nejprve vyhledat existující hodnotu a zobrazí chybu v případě snahy zapsat neunikátní hodnoty; tento příznak se využívá v případě nastavení primárních klíčů</li> <li>Filtr rozsahu – lze nastavit jen u NON CLUSTERED, protože CLUSTERED musí vždy obsahovat všechna data – dovoluje omezit podle podmínky pro které řádky se bude index vytvářet; používá se pro optimalizaci výkonu při zápisu a velikosti indexu na disku</li> <li>Included columns – lze nastavit jen u NON CLUSTERED, protože CLUSTERED již obsahuje všechny sloupce – dovoluje přidat sloupce, které budou součástí datových stránek u indexu</li></ul> <h2> <h2>Obsah indexů na disku</h2></h2> <p><strong>Hlavní uložiště (CLUSTERED INDEX nebo HEAP) obsahuje vždy všechny sloupce a řádky tabulky.</strong> Dá se brát jako hlavní referenční uložiště řádků tabulky, na které se všechny další NON CLUSTERED indexy odkazují. Existují tyto 3 varianty, jak NON CLUSTERED indexy odkazují na řádky v hlavním indexu:</p> <ul> <li>Pokud je hlavní uložiště HEAP, uchovává se v datové části NON CLUSTERED indexu přímý odkaz na pozici v datovém souboru (index stránky a pozice na stránce) – to je ale nepříjemný fakt z pohledu výkonu, protože každá reorganizace stránek na disku nebo záznamů na stránce vyžaduje úpravy i všech NON CLUSTERED indexů aby pointery na datové stránky odkazovali na správné místo; to je hlavní důvod, proč nepoužívat typ HEAP</li> <li>Pokud je hlavní uložiště CLUSTERED INDEX s příznakem UNIQUE, odkazují se na něj všechny NON CLUSTERED indexy právě přes tuto hodnotu, protože je unikátní a jednoznačně identifikuje řádek, který je možné pomocí tohoto klíče rychle dohledat; toto je zároveň důvod proč se nedoporučuje používat velké CLUSTERED INDEX sloupce, jejich hodnota se kopíruje i do datových stránek NON CLUSTERED indexů jako identifikace řádku, který reprezentují</li> <li>Pokud je hlavní uložiště CLUSTERED INDEX bez příznaku UNIQUE, chová se odkazování podobným způsobem, až na rozdíl, kdy se vyskytnou dva záznamy s duplicitou ve sloupcích nastavených v CLUSTERED indexu – v té chvíli interně SQL Server přidá sloupci ještě 4B identifikátor čísla řádku, aby ho byl schopný jednoznačně identifikovat</li></ul> <p>Všechny NON CLUSTERED indexy tedy v datové části obsahují:</p> <ul> <li><strong>Sloupce, podle kterých jsou seřazeny</strong></li> <li><strong>Sloupce, podle kterých je seřazen hlavní CLUSTERED INDEX</strong> (+4B identifikátor, pokud není hodnota unikátní) nebo přímý odkaz na specifickou stránku v případě HEAP</li> <li><strong>Sloupce, které jsou přidány pomocí nastavení <em>include columns</em></strong></li></ul> <h2>Sestavování dotazů</h2> <p>Pokud spustíme dotaz proti tabulce, SQL Server se snaží najít nejlepší způsob, jak jej vykonat vzhledem k indexům a nastavení tabulky. Obvykle si lze logicky odvodit, jak se server zachová. Zároveň lze v SSMS zobrazit tlačítkem <em>Display Estimated Execution Plan</em> / <em>Include Actual Execution Plan</em> zobrazit předpokládaný/skutečný plán vykonání dotazu.</p> <p>Zde je velmi důležité znát indexy na tabulce a jejich nastavení. Při analýze dotazů ignorujte, zda se jedná o CLUSTERED nebo NON CLUSTERED – zaměřte se jen na sloupce, které obsahují a podle jakých sloupců jsou seřazené. V některých případech je samozřejmě dobré vzít v potaz i další konfiguraci, ale pro základ stačí jejich sloupce a řazení.</p> <p>Při čtení obvykle nalezneme tyto typy čtení:</p> <ul> <li>INDEX SCAN / TABLE SCAN – sekvenční čtení celé nebo části indexu nebo HEAP od začátku</li> <li>INDEX SEEK – vyhledání konkrétního záznamu nebo rozsahu záznamů pomocí stromu indexu</li> <li>KEY LOOKUP – byl použitý NON CLUSTERED index pro vyhledání identifikátoru a dotahují se hodnoty ostatních sloupců z jiného indexu (obvykle CLUSTERED)</li></ul> <h3>Ukázkový dotaz</h3> <p>Jako modelovou situaci mějme tabulku [Customer]:</p> <ul> <li>Má sloupce [Id] INT, [Company] NVARCHAR(500), [Name] NVARCHAR(500), [Email] VARCHAR(300)</li> <li>Unikátní CLUSTERED INDEX na sloupci [Id]</li> <li>NON CLUSTERED index na sloupci [Company] s included column [Name]</li></ul> <p>Zvažte následující dotazy a vysvětlení jejich exekučního plánu:</p> <p><font face=""Consolas"">SELECT [Id], [Company], [Name], [Email] FROM [Customer]</font></p> <ul> <li>Je potřeba načíst všechny sloupce a všechny řádky</li> <li>Použije se CLUSTERED INDEX, který obsahuje všechny data a provede se INDEX SCAN (sekvenční čtení indexu) přes celou tabulku.</li></ul> <p><font face=""Consolas"">SELECT [Id], [Company], [Name], [Email] FROM [Customer] WHERE [Id] = 123</font></p> <ul> <li>Je potřeba načíst všechny sloupce a jeden řádek (protože server ví, že na [Id] je unikátní index a výsledek bude jen jen)</li> <li>Proto se použije CLUSTERED INDEX, který obsahuje všechny data </li> <li>Provede se INDEX SEEK (vyhledání ve stromu indexu) jednoho konkrétního řádku podle [Id] sloupce.</li></ul> <p><font face=""Consolas"">SELECT [Id], [Company], [Name], [Email] FROM [Customer] WHERE [Name] = ‘John Smith’</font></p> <ul> <li>Je potřeba načíst všechny sloupce a neznámý počet řádků (server neví, kolik Johnů Smithů je v tabulce)</li> <li>Proto se použije CLUSTERED INDEX, který obsahuje všechny data</li> <li>Provede se INDEX SCAN (sekvenční čtení indexu) pro prohledání celé tabulky a nalezení všech výskytů.</li></ul> <p><font face=""Consolas"">SELECT [Id] FROM [Customer] WHERE [Name] = ‘John Smith’</font></p> <ul> <li>Je potřeba načíst pouze sloupce [Id] a [Name]</li> <li>To dokáže plně pokrýt index na sloupci [Company] – ten totiž obsahuje [Id] jako odkaz do hlavního indexu a zároveň [Name] jako included column</li> <li>SQL Server preferuje vždy čtení datově menšího indexu (obsahující méně sloupců) před větším, protože není potřeba číst tolik stránek z disku</li> <li>Ve výsledku se tedy provede INDEX SCAN a projde se celý NON CLUSTERED index. U odpovídajících řádků přečte [Id] a není vůbec nutné číst z hlavního indexu.</li></ul> <p><font face=""Consolas"">SELECT [Id], [Company], [Name], [Email] FROM [Customer] WHERE [Company] = ‘dotNETcollege’</font></p> <ul> <li>Je potřeba načíst všechny sloupce a neznámý počet řádků (server neví, kolik zákazníků z dotNETcollege je v tabulce)</li> <li>Pravděpodobně bude použitý NON CLUSTERED INDEX pro vyhledání všech [Id] záznamů podle příslušného WHERE predikátu</li> <li>Dále se provede KEY LOOKUP do hlavního CLUSTERED INDEX, kde se postupně podle všech [Id] načtou zbývající sloupce</li></ul> <h3>Kdy SQL Server nepoužije NON CLUSTERED INDEX?</h3> <p>Může se stát, že SQL Server se rozhodne nepoužít vámi vytvořený index a raději zvolí na první pohled nepříliš efektivní SCAN celého jiného indexu. Nejčastější důvody jsou:</p> <ul> <li>Očekáváte KEY LOOKUP, protože filtrujete podle sloupce v indexu a místo toho je použitý SCAN hlavního indexu? Pravděpodobně server usoudil, že výraz WHERE predikátu by vrátil příliš mnoho záznamů a kdyby pro každý z nich dohledával řádek v hlavním indexu, bude to příliš náročné a raději projde celou tabulku.</li> <li>Pokud nebyl použitý očekávaný index, možné je jen v tabulce zatím příliš málo řádku a v tu chvíli raději SQL Server přečte celou tabulku, než aby dohledával záznamy přes strom indexu. Vždy je totiž nejmenší objem dat, se kterým se pracuje, celá stránka a proto je často sekvenční čtení výhodnější.</li></ul> <h2>Primární klíče</h2> <p>Na závěr bych rád zdůvodnil, proč jsem v celém článku nezmínil primární klíče. Primární klíč je totiž víc formální záležitost, kterou lze plně zastoupit běžným indexem, než funkce navíc. </p> <p>Pokud na tabulce založíme primární klíč, je to interně běžný index, který omezuje nastavení na:</p> <ul> <li>Lze vytvořit jen jeden na tabulku</li> <li>Je unikátní</li> <li>Nedovoluje aplikovat filtr (aby vždy pokryl všechny řádky)</li> <li>Nedovoluje nastavit included columns</li> <li>Můžeme si alespoň zvolit, zda bude uložen jako CLUSTERED nebo NON CLUSTERED index.</li></ul> <p>Pojmy CLUSTER INDEX a PRIMARY KEY se často pletou, protože většina tabulek má primární klíč nastavený právě nad CLUSTERED indexem – obvykle sloupce [Id] a podobně a proto pojmy splývají.</p> <p>Závěrem tedy doporučuji při optimalizacích a ladění zcela vypustit pojem primárního klíče a orientovat se pouze na fyzické indexy a jejich nastavení. Stejně to dělá i SQL Server a je mu zcela jedno, zda je na sloupci unikátní index nebo primární klíč.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8509, 3, CAST(N'2017-01-01T17:25:51.923' AS DateTime), N'Na co se můžete těšit v roce 2017?', N'<p>Rok 2016 byl pro mě a pro <strong><a href=""http://www.riganti.cz"">RIGANTI</a></strong> velmi důležitý a doposud asi nejnáročnější, co jsem zažil. </p> <p>&nbsp;</p> <h3>DotVVM</h3> <p>Pořádně jsme pohnuli s <a href=""https://www.dotvvm.com""><strong>DotVVM</strong></a>, naším open source frameworkem, který šetří čas při psaní složitých webových aplikací. V červenci jsme vypublikovali první ostrou verzi <strong>1.0</strong>, aktuálně máme skoro hotovou <strong>1.1</strong>, která podporuje kromě <strong>OWIN</strong>u i <strong>ASP.NET Core</strong>, takže svou aplikací nejste vázáni na starou platformu a můžete celkem bez obtíží v budoucnu aplikaci zmigrovat a provozovat ji třeba na Linuxu.</p> <p>Připravujeme také uvedení našeho komerčního balíku komponent <strong><a href=""https://www.dotvvm.com/landing/business-pack"">DotVVM Business Pack</a></strong>, první verze bude uvolněna koncem ledna a bude obsahovat pokročilé gridy a mnoho komponent, které se hodí při tvorbě velkých informačních systémů.</p> <p>&nbsp;</p> <h3>DotNetPortal a DotNetCast</h3> <p>Mnohem více úsilí v novém roce bych chtěl věnovat do <strong>DotNetPortal</strong>u, protože posledních několik let jsem neměl moc času psát články a o web se nějak systematičtěji starat.</p> <p>Kromě toho bych chtěl rozjet nový web <strong>DotNetCast</strong>, který bude linkovat zajímavá videa o .NETu a MS technologiích. Kromě toho, že budeme z různých zdrojů sbírat to, co nás zaujme, počítáme i s tvobrou vlastního obsahu. V plánu je udělat stejnojmenný seriál, kde vám každý týden přestavíme nějakou zajímavou technologii a ukážeme, k čemu slouží a jak začít.</p> <p>&nbsp;</p> <h3>GeekCore</h3> <p>Kromě toho jsme s <a href=""http://www.aspnet.cz"">Altair</a>em začali přepisovat web <a href=""https://www.geekcore.cz""><strong>GeekCore</strong></a> do modernější podoby, protože je praktické sledovat jedno místo a mít přehled o všech tech akcích, které se v ČR konají.</p> <p>&nbsp;</p> <h3>Nové kurzy DotNetCollege</h3> <p>Pozadu nezůstane ani <strong><a href=""https://www.dotnetcollege.cz"">DotNetCollege</a></strong>. Kromě <strong>kurzů a školení pro vývojáře a firmy</strong>, které nyní pořádáme <strong>v Praze</strong> i <strong>v Brně</strong>, plánujeme pořádat 3 menší konference ročně. Loni jsme takto již zorganizovali konference <a href=""https://www.secpublica.cz/2016/""><strong>SecPublica</strong></a> a <a href=""https://www.corestart.cz/""><strong>CoRestart</strong></a>.&nbsp; </p> <p>Na únor připravujeme konferenci o architektuře softwaru, a to jednak z pohledu návrhu samotné infrastruktury, tak i bezpečnosti. </p> <p>Na druhou polovinu roku 2017 pak máme v plánu uspořádat velkou a placenou konferenci o <strong>.NET</strong>u a <strong>Azure</strong>, kam chceme pozvat převážně zahraniční speakery, kteří u nás nejsou “okoukaní”. Konference bude v angličtině a myslím, že to bude velmi zajímavé, mít možnost vidět naživo lidi z Microsoftu nebo známé MVP z různých koutů Evropy. </p> <p>&nbsp;</p> <p>Pokud si nechcete nic z toho nechat ujít, určitě sledujte DotNetPortal, nebo mě followujte na <a href=""https://twitter.com/hercegtomas""><strong>Twitter</strong></a>u. </p> <p>A pokud byste se chtěli <strong>spolupodílet</strong> na některém z těchto <strong>zajímavých projektů</strong>, určitě mi dejte vědět – hledáme jak programátory do <a href=""http://www.riganti.cz/jobs"">RIGANTI</a>, tak lektory pro <a href=""https://www.dotnetcollege.cz"">DotNetCollege</a>.</p> <p><strong>Přeji vám hodně štěstí a úspěchů v novém roce 2017.</strong></p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8510, 7, CAST(N'2017-01-02T00:58:55.927' AS DateTime), N'Pozvánka na veletrh práce pro programátory Jobs Dev 2017', N'<p><em>Reklamní sdělení</em></p><p>Pojďte se potkat s dalšími 2 500 programátory z celé ČR v sobotu 8. dubna v Praze na Fakultě architektury spolu s 200 firmami a několika programátorskými komunitami. Potkáte zde nové lidi, možná najdete komunitu, o které jste nevěděli. Zjistíte, jaké mají nároky velké firmy a zda by o vás měli zájem, nebo si s nimi rovnou na veletrhu domluvíte pohovor.  <p>Veletrh bude doprovázený několika paralelními proudy přednášek. Pro každý z hlavních jazyků (Java, PHP, JavaScript, C++, C#, SQL) budou vždy 2-3 přednášky a jedna velká soutěž, kde můžete změřit své síly a znalosti s ostatními programátory. Pro méně rozšířené jazyky či platformy (Python, .NET, HbbTV, embed systémy a další) bude na veletrhu prostor pro jednotlivé přednášky a workshopy, pokud o ně ze strany komunity bude zájem. <p>Kromě cen z těchto soutěží se budete moci zúčastnit slosování bezpočtu menších soutěží u každé firmy o hmotné ceny jako telefony, tablety, hardware a další. <p>Pro aktuální informace o veletrhu sledujte web <a href=""http://www.jobsdev.cz"">jobsdev.cz</a> nebo Twitter <a href=""https://twitter.com/jobsdevcz"">@jobsdevcz</a> či <a href=""https://www.facebook.com/jobsdevcz/"">FB stránku</a>.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8511, 15, CAST(N'2017-01-23T12:51:36.033' AS DateTime), N'Využití C# Scripting', N'<p>Možná jste se jako já dostali do situace, že jste potřebovali vyzkoušet nějaký kus C# kódu na produkčním prostředí nebo někde jinde, kde prostě není (a není dobré instalovat) Visual Studio a vývojové nástroje. Důvodem může být to, že na produkčním prostředí se kód chová jinak, nebo na vývojovém prostředí nelze nasimulovat stejné podmínky. Tento problém řešíme například tak, že si napíšeme nějakou Consolovku nebo formulářovou aplikaci s testovací tlačítkem apod., a nebo nově využijeme C# Scripting.</p> <h3>C# Scripting</h3> <p><strong>C# Scripting</strong> je možnost jak psát scripty v jazyce C#. Vše je uděláno díky <strong>.NET Compiler Platform (""Roslyn"")</strong>, a jako celý Rozlyn, je i C# Scripting opensource. Script, který se píše do souboru s příponou <strong>.csx</strong>, je tedy zapisován v jazyce C#, je zde ale pár změn oproti klasickému .cs. C# příkazy můžeme psát přímo, nemusí být umístěny ve třídě. Druhou hlavní změnou je to, že v C# scriptu nejsou namespaces. Tyto změny jsou z důvodu, aby se psaní scriptů co možná nejvíce zjednodušilo. Zde vidíme jednoduchý příklad C# a C# Script:</p> <table cellspacing=""0"" cellpadding=""0"" width=""875"" border=""0""> <tbody> <tr> <td valign=""top"" width=""415""><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello World!"");
    }
}</pre></td>
<td valign=""top"" width=""458""><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">using System;

Console.WriteLine(""Hello World!"");
</pre></td></tr></tbody></table>
<h3>C# Interactive</h3>
<p>Další součástí C# Scripting je <strong>command-line nástroj</strong> <strong>csi</strong> - <strong>CSharp Interactive</strong>. Jedná se o tzv. <strong>REPL</strong> (read eval print loop) nástroj, který umožňuje zadávat C# příkazy a rovnou je po odeslání vykonávat. Csi dále umožňuje psát některé speciální vestavěné příkazy jako <strong>#help</strong> – nápověda, <strong>#r</strong> – načtení reference assemby, atd. Csi.exe je součástí instalace <strong>MSBuild</strong>. Spustit ho na windows (csi je napsán v .NET Core, takže je dostupný i na Linux, MacOS) můžeme buď přímo z <strong>Developer Command Prompt</strong>, nebo z cesty “<em>C:\Program Files (x86)\MSBuild\14.0\Bin\</em>” (pro VS2015).</p>
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Vyuit-C-Scripting_FC52/image_3.png"" width=""576"" height=""200""></p>
<p>Po spuštění csi, již máme <strong>načtené některé reference a některé using importy</strong> jako <em>using System</em> a další, a tak můžeme rovnou použít např. třídu <em>Console</em>. Definice těchto referencí a using je uvedena v tzv. Response souboru (pro csi konkrétně <em>C:\Program Files (x86)\MSBuild\14.0\Bin\csi.rsp</em>).</p>
<p>Csi také umožňuje <strong>spouštět celé .csx script</strong> soubory, spuštěním příkazu #load, nebo uvedením .csx souboru při spouštění csi:<br><em>csi.exe script-file.csx</em></p>
<h3>Visual Studio C# Interactive Window</h3>
<p>Součástí Visual Studia (od VS2015 Update 1) máme nové <strong><a href=""https://github.com/dotnet/interactive-window/"" target=""_blank"">C# Interactive window</a></strong>, které obsahuje to samé REPL prostředí jako csi. Díky tomu, že je ale postaveno na Visual Studio editoru, tak obsahuje integrovaný <strong>IntelliSense</strong>, <strong>syntax-coloring</strong> atd., podobně jako editor .csx souborů.</p>
<p><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Vyuit-C-Scripting_FC52/image_6.png"" width=""653"" height=""230""></p>
<p>Pokud máme ve VS otevřený .csx script, můžeme jeho příkazy spouštět v Interactive window, pomoci volby <strong>Execure in interactive (Ctrl+E, Ctrl+E)</strong>.</p>
<p>Více o C# Interactive si můžete prostudovat např. zde:<br><a title=""https://github.com/dotnet/roslyn/wiki/C%23-Interactive-Walkthrough"" href=""https://github.com/dotnet/roslyn/wiki/C%23-Interactive-Walkthrough"" target=""_blank"">https://github.com/dotnet/roslyn/wiki/C%23-Interactive-Walkthrough</a><br><a title=""http://dailydotnettips.com/2016/01/12/use-c-interactive-window-for-your-coding-experiment-in-visual-studio-2015/"" href=""http://dailydotnettips.com/2016/01/12/use-c-interactive-window-for-your-coding-experiment-in-visual-studio-2015/"" target=""_blank"">http://dailydotnettips.com/2016/01/12/use-c-interactive-window-for-your-coding-experiment-in-visual-studio-2015/</a><br><a title=""https://msdn.microsoft.com/en-us/magazine/mt614271.aspx"" href=""https://msdn.microsoft.com/en-us/magazine/mt614271.aspx"" target=""_blank"">https://msdn.microsoft.com/en-us/magazine/mt614271.aspx</a></p>
<h3>Spuštění C# scriptu</h3>
<p>Zpátky ale k našemu využití tj. máme napsaný C# script s testovacím kódem a potřebujeme ho spustit na nějakém jiném než vývojovém prostředí, kde nemáme VS ani C# Interactive (csi) nainstalován.</p>
<p>Jedna z možností je ručně si nahrát na tento počítač již zmíněný <strong>tool csi</strong> a script spustit pomoci něho. Jediná prerequisita je plný <strong>.NET Framework</strong> alespoň <strong>v 4.6</strong> nebo vyšší. Potom stačí do nějakého adresáře nahrát následující soubory z vývojového počítače z “<em>C:\Program Files (x86)\MSBuild\14.0\Bin</em>\”:</p>
<p><em>csi.exe<br>csi.rsp<br>Microsoft.CodeAnalysis.CSharp.dll<br>Microsoft.CodeAnalysis.CSharp.Scripting.dll<br>Microsoft.CodeAnalysis.dll<br>Microsoft.CodeAnalysis.Scripting.dll<br>System.AppContext.dll<br>System.Collections.Immutable.dll<br>System.IO.FileSystem.dll<br>System.IO.FileSystem.Primitives.dll<br>System.Reflection.Metadata.dll</em></p>
<p>a spustit csi.exe.</p>
<h3>C# Scripting API</h3>
<p>Druhou možností je využít <strong>C# Scripting API</strong>. C# Scripting má totiž ještě jednu součást, o které zatím nebyla řeč, a tou je programové <strong>Hosting API</strong>. To nám umožňuje hostovat C# script engine a přes něj z našeho programu spouštět kusy <strong>C# Script</strong> kódu.</p>
<p>Pro použití potřebujeme nainstalovat NuGet balíček <a href=""https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Scripting"" target=""_blank"">Microsoft.CodeAnalysis.CSharp.Scripting</a>:<br><em><strong>Install-Package Microsoft.CodeAnalysis.CSharp.Scripting</strong></em></p>
<p>Dále použijeme <strong><em>using Microsoft.CodeAnalysis.CSharp.Scripting</em></strong>, ve kterém je umístěna třída <em><strong>CSharpScript</strong></em>. Pomoci ní již můžeme vyhodnocovat C# výrazy (EvaluateAsync), nebo zkompilovat a spustit C# script (RunAsync). Příklady jsou uvedeny na stránce <a href=""https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples"" target=""_blank""><strong>Scripting API Samples</strong></a>.</p>
<p>S využitím C# Scripting API jsem si napsal vlastní jednoduchou WPF aplikaci, která umožnuje spustit libovolný <strong>C# Script</strong> kód.</p>
<p><img title=""image"" style=""border-top: 0px; border-right: 0px; background-image: none; border-bottom: 0px; padding-top: 0px; padding-left: 0px; border-left: 0px; display: inline; padding-right: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Vyuit-C-Scripting_FC52/image_12.png"" width=""445"" height=""301""></p>
<p>Hlavní kód aplikace je obsluha tlačítka <strong>Run</strong> pro spuštění scriptu a odchycení výsledků z console do textboxu <strong>txtConsole</strong>. Kód je umístěn v souboru <strong>MainWindow.cs</strong>:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">using System;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace CSharpScripting
{
    #region ControlWriter
    public class ControlWriter : TextWriter
    {
        private readonly TextBox textbox;

        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
    #endregion

    /// &lt;summary&gt;
    /// Interaction logic for MainWindow.xaml
    /// &lt;/summary&gt;
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Console.SetOut(new ControlWriter(txtConsole));

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.Keyboard.Focus(txtScript);
        }

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;
            this.Cursor = System.Windows.Input.Cursors.Wait;

            try
            {
                txtConsole.Text = """";

                var scriptRunner = CSharpScript.Create(txtScript.Text, GetScriptOptions()).CreateDelegate();
                await scriptRunner.Invoke();
            }
            catch (Microsoft.CodeAnalysis.Scripting.CompilationErrorException ex)
            {
                txtConsole.Text = txtConsole.Text + (txtConsole.Text.Length == 0 ? """" : Environment.NewLine) + string.Join(Environment.NewLine, ex.Diagnostics);
            }
            catch (Exception ex)
            {
                txtConsole.Text = txtConsole.Text + (txtConsole.Text.Length == 0 ? """" : Environment.NewLine) + string.Join(Environment.NewLine, ex.ToString());
            }
            finally
            {
                ButtonRun.IsEnabled = true;
                this.Cursor = null;
            }
        }

        private Microsoft.CodeAnalysis.Scripting.ScriptOptions GetScriptOptions()
        {
            return Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default
                .WithReferences(typeof(System.Diagnostics.Process).Assembly,            //System Assembly
                                typeof(System.Dynamic.DynamicObject).Assembly,          //System.Core Assembly
                                typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly) //Microsoft.CSharp Assembly
                .WithImports(new[] {
                    ""System"",
                    ""System.IO"",
                    ""System.Collections.Generic"",
                    ""System.Diagnostics"",
                    ""System.Dynamic"",
                    ""System.Linq"",
                    ""System.Linq.Expressions"",
                    ""System.Text"",
                    ""System.Threading.Tasks""
                });
        }
    }
}</pre>
<p>Povšimněte si funkce <strong>GetScriptOptions</strong>, která se stará o to, aby vrátila nastavení pro CSharpScript.Create. Zde řeším přidání <strong>některých referencí a using importů</strong>, aby byly automaticky pro script k dispozici (podobně jako v csi nebo VS C# Interactive).</p>


<p>Zdrojové soubory celé aplikace i zkompilovanou release verzi naleznete na GitHubu <a href=""https://github.com/holajan/CSharpScripting"" target=""_blank""><strong>holajan/CSharpScripting</strong></a>.</p>
<hr>
Jako zdroj zabývající se C# scripting dále doporučuji první část přednášky přímo od jednoho z autorů C# scriptingu:<br><a href=""https://www.youtube.com/channel/UCyu-SSbp_10RTJyjCn6K_oQ"" target=""_blank"">DotNetCast</a> - <a href=""https://www.youtube.com/watch?v=oIbQV5tCGvk&amp;t=1998s"" target=""_blank""><strong>Tomáš Matoušek: Interactive Development with C#</strong></a><strong> </strong>(v CZ)<strong>.</strong>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8520, 22, CAST(N'2017-02-26T18:34:21.917' AS DateTime), N'Možnosti Webového vývoja na .NET Core', N'<p>Jedna z tém, ktoré sa budú v blízkej budúcnosti asi často objavovať je .NET Core. Jeho multiplatformovosť je veľká výhoda a vo väčšine príkladov Microsoft spomína jeho uplatnenie ako platforma pre nenáročný a spoľahlivý webserver hostovaný napríklad na Linuxe. </p> <p>Microsoft ale s príchodom .NET Core ohlásil, že ďalej nebude podporovať ASP.NET Web Forms. WebForms má dni svojej slávy už jednoznačne za sebou, ale v Strednej Európe je tento framework stále obľúbený a mnoho veľkých firiem má na ňom postavené svoje interné informačné systémy a webové aplikácie. Podporovaný ale nebude a na .NET Core tým pádom prichádza do úvahy len použiť prezentačný framework ASP.NET MVC, ktorý zostal ako jediná podporovaná webová technológia.  <p>ASP.NET MVC 6, na .NET Core nazvané ASP.NET MVC 1.0 Core prináša niekoľko veľmi príjemých zmien. Aj keď je pomerne komplexný, stále dovoľuje veľkú flexibilitu pri návrhu architektúry webovej aplikácie. Od komplikovaných Controllerov, ktoré sa starajú takpovediac o každý aspekt správania a vzhľadu stránky, až po minimalistické WebAPI, ktoré podporuje základné CRUD operácie a zbytok aplikácie tvorí HTML/Javascript. Už dlhšiu dobu naberajú na popularite javascriptové frameworky ako AngularJS, Knockout, React a Vue a celá zoo ďalších. Tieto frameworky ale žiadnu extra prezentačnú vrstvu na strane servera nepotrebujú, naviac server-side logiku zastane napríklad Node.js. Výsledkom je, že C# akoby vo webovom prostredí strácal pozíciu v prospech JavaScriptu. Oproti tejto konkurencii sa ASP.NET MVC snaží prinášať nové vymoženosti, ktoré by .NET vývojárov&nbsp; zaujali a uľahčili im život.  <p>Na druhej strane barikády stojí Tomáš Herceg so svojou firmou Riganti, ktorí majú za cieľ ponúknuť alternatívu k MVC. Hľadali nástroj, z ktorého pomocou by dokázali vyvíjať užívateľské rozhrania podobne pohodlne ako vo WPF a naviac bol zrozumiteľný pre programátorov zvyknutých na ASP.NET Web Forms. Svoj framework DotVVM postavili na návrhovom vzore MVVM. Jeho základnú štruktúru tvorí View-model napísaný na strane servera v C# a view v HTML obohatenom o koncept data-bindingov. </p> <p>MVC je už rokmi vyskúšaný framework, ktorý Microsoft vylepšil a priniesol ako prvú voľbu webového vývoja na .NET Core. Naproti tomu DotVVM je novinka, ktorá je vo vývoji necelé dva roky a zdanlivo ide proti prúdu: snaží sa minimalizovať písanie JavaScriptu a má plnú podporu pre .NET Core. Samozrejme celý kód je otvorený a do projektu je možné prispieť na GitHube. Myslím, že stojí za to si ho predstaviť ako sľubnú alternatívu. </p> <h2>Štruktúra projektu</h2> <p><img alt=""porovnanie projektov MVC vs. DotVVM"" src=""https://lh4.googleusercontent.com/vlmfy92EpFaYFUGxIETmCqwHa2kUH5rXA190TjSpXFF8Rua1cYdhXgzmzqnMv5KamEYLpN0VncsOGNrQZAEnlHciJ88bs7R8rON8GVoUx_5nYZV5I3nO1JgUpi8l3Ig45Q"" width=""507"" height=""520""> </p> <p>Oba projekty majú štruktúru, typickú pre .NET Core webový projekt. Rozdelenie do adresárov zodpovedá použitej architektúre. <span style=""font-family: courier new; background-color: #ddd"">Program.cs</span> a trieda <span style=""font-family: courier new; background-color: #ddd"">Startup</span> je v obidvoch zhruba rovnaký až na jeden riadok, ktorý nastaví príslušný framework ako middlewere. Pri DotVVM je špecifická konfigurácia vyčlenená do samostatnej triedy, MVC má svoju špecifickú konfiguráciu priamo v súbore <span style=""font-family: courier new; background-color: #ddd"">Startup.cs</span>. MVC používa ako svoje views súbory s príponou cshtml ktoré sú písané v markupovom jazyku Razor, DotVVM svoje <span style=""font-family: courier new; background-color: #ddd"">.dothtml</span> views&nbsp; písané vo vlastnom obohatenom HTML. </p> <p>Najväčší rozdiel je v tom, ako je v oboch frameworkoch riešený code-behind. Základným stavebným kameňom architektúry MVC je Controller. Je to bezstavová trieda, kde každá metóda odsluhuje jedno volanie na webserver. Jeden Controller obsluhuje viacero stránok a&nbsp; združuje metódy obsahujúce podobný typ požiadavkov. Metóda príjme dáta poslané na server, spracuje ich, vyberie a vráti správne view, ktorému poskytne dáta na zobrazenie. Controller samotný stavové zmeny na stránke neuchováva, ide o komunikáciu “požiadavka - odpoveď”. Akékoľvek stavové informácie musí programátor “ručne” získať z požiadavky na server.  <p>Naproti tomu DotVVM využíva výhody MVVM. Jeden view-model obsluhuje pri požiadavke na server len jednu stránku. DotVVM View je so svojím view-modelom pevne zviazané. View-model si naprieč požiadavkami uchováva kompletný stav svojej stránky. Takto združuje nielen dáta (properties) s ktorými stránka pracuje, ale aj funkcie. Tie zodpovedajú operáciám, ktoré možno na stránke vykonať, napríklad klik na tlačidlo. Ak užívateľ vykoná na stránke operáciu a spustí sa funkcia vo view-modely, táto funkcia má k dispozícií všetky dáta view-model triedy už pripravené tak, ako ich vidí užívateľ na stránke. </p> <h2><font style=""font-weight: normal"">Mapovenie ciest</font></h2> <p><img alt=""možnosti mapovania ciest v MVC"" src=""https://lh3.googleusercontent.com/dWiZXzzZWTV73jvPALHwgsGbhUTd00r7OqvDrsiOXWDvydnY-pzx1dOzdUpbktV5PZIF81WhBYR1gwF4q9Grz-pdQww_pp8YauRaEx_1s0BCuipfPyx22qA2rrcB2kiCKg"" width=""457"" height=""485""> </p> <p>ASP.NET MVC Core má bohaté možnosti konfigurácie mapovania požiadaviek na server na akcie príslušných Controllerov. Je možné nastaviť mapovanie priamo v triede <span style=""font-family: courier new; background-color: #ddd"">Startup.cs</span> a držať si tak celú konfiguráciu ciest pokope. Tiež je možné jednotlivým Controllerom predpísať ich cestu atribútom. To isté platí pre akcie. Platí že atribúty akcií majú väčšiu váhu, ako atribúty Controllera.&nbsp; Taktiež je možné v atribúte použiť parametre <span style=""font-family: courier new; background-color: #ddd"">[controller]</span> a <span style=""font-family: courier new; background-color: #ddd"">[action]</span>, ktoré dosadia meno aktuálneho Controllera a akcie. </p> <p><img alt=""konfigur&aacute;cia ciest v DotVVM"" src=""https://lh5.googleusercontent.com/0VjZhOfJ6DAuuqK9BpttxxxBqWzi064K7pKg4Eqv-zHLAej05MqC1e6Li0qbmEnUGmA9ugL2n7Ox0sjlXnCEdOOXwGuPJLBsKCyj8O2Nvj_lw5cz-DDSAe2KWmzH5ax54w"" width=""602"" height=""120""> </p> <p>Jediná možnosť ako v DotVVM nastaviť mapovanie ciest, je priradiť pomenovanú cestu konkrétnej .dothtml stránke v konfigurácií DotVVM a tak spárovať požiadavku na server so stránkou, ktorá sa má spracovať ako odpoveď. Pridávať cesty ručne by bolo na veľkých weboch pomerne nepraktické, našťastie je možné využiť možnosť rozšíriteľného auto-discovery, ktorá na&nbsp; základe konkrétnej stratégie .dothtml stránky nájde a namapuje na cesty. Základná stratégia namapuje stránky v priečinku “Views” podľa podpriečinkov. </p> <p><img alt=""pr&iacute;stup k parametrom z adresy MVC vs. DotVVM"" src=""https://lh5.googleusercontent.com/e5_XUZOgCSoaAx-qRH2i6dsV12zjAekgsKZDENntK9k4s_ZYCKy744XomytwonT2aWW39j_7AYwrJMvJRwnkWx4ueEVLEz_vTTaNiv69pbaLLj-CIvHGT47Ols-b6xHAAw"" width=""602"" height=""123""> </p> <p>Aj DotVVM aj MVC podporujú premenné parametre v adrese. Stačí ich uviesť v konfigurácií cesty a oba frameworky sa už postarajú o ich správne mapovanie. </p> <p>V MVC prídu nachystané so správnym typom do parametrov akcií Controlleru podľa mena a atribútu <span style=""font-family: courier new; background-color: #ddd"">[FromRoute]</span>. V DotVVM view-modely ich treba získať zo slovníka Parameters objektu Context, ktorý je dostupný na štandardných DotVVM view-modeloch. Správny typ, dostupnosť a názov parametru si ale musí programátor postrážiť sám. </p> <h2>Views</h2> <p><img alt=""Razor uk&aacute;žka k&oacute;du"" src=""https://lh3.googleusercontent.com/LFIIClr1h5bJWdscZpHBDO6xtZ0HATmsnZZTKjkEL0-yWG5I2xZ4uj2aSjFnSTlbQcK67Q_KuJYZa1uU0Blg5waQsz8c-9jX6y0OUubjCkTU6KhnYpNG6FnwVMk73-I2ZQ"" width=""602"" height=""455""></p> <p> <p>Filozofia písania views v MVC by sa dala zhrnúť slovom minimalizmus. Dať programátorovi sadu základných funkcií a nechať ho nech si funkcionalitu stránok tak povediac “od podlahy” navrhne sám. Razor využíva priamu kombináciu C# kódu a HTML, nič neskrýva a tagy vykresluje jedna k jednej. Pri zostavovaní výsledného HTML sa dajú používať takmer všetky konštrukcie zo C# vrátane podmienok a cyklov. Dáta poskytnuté Controllerom na vykreslenie sú prístupné vo vstavanej premennej Model alebo slovníku <span style=""font-family: courier new; background-color: #ddd"">ViewData</span>. Na najčastejšie používané HTML konštrukcie existujú špeciálne funkcie - HTML helpery, ktoré podľa zadaných parametrov vykreslia HTML do stránky. Ide o jednoduchý priamočiary nástroj bez vnútornej logiky a väčšinou vykresľuje konkrétny HTML tag.  <p>Znovupoužiteľnosť a prenositelnosť kódu zaisťuje špeciálny typ views - Partial views. Ide o typ view, ktorého úloha je vykresliť len časť stránky. Partial view má vlastný model a <span style=""font-family: courier new; background-color: #ddd"">ViewData</span>. Partial view je možné vykresliť do ľubovoľného iného view, a predať mu kompatibilné dáta. Je možné tiež vrátiť Partial view ako výsledok volania na Controller a dá sa takéto volanie vynútiť pri samotnom zostavovaní view v ktorom sa Partial view používa. Tento mechanizmus je veľmi dobre možné využiť na vytváranie prenesiteľných znovu použiteľných sád Partial view. Views sa zostavujú ako textové súbory priamo bez akejkoľvek klientskej logiky. Nevýhodou je že pri zmene dát alebo požiadavke na server (napr.: odoslaní formulára) dát sa musí spraviť plnohodnotné volanie na server a celá stránka sa musí prekresliť s novými dátami. Dnes takéto priame vykresľovanie chce málokto a logika sa deleguje z Controllera na JavaScript na strane klienta s čím ale ASP.NET MVC príliš nepomôže.  <p><img alt=""DotVVM markup uk&aacute;žka k&oacute;du"" src=""https://lh3.googleusercontent.com/FJb_K0lHUsZpHKTVd5tvGXXEzeBv_hg8lVIoPgSMBgXVSUua3ZZXvl9bgvv0MtOItf6z-ZIVSixQXizVahgZtIebIBbLI1DqRgFOF9JWp9vEw44JGVbydRe5UoyiIo-zEA"" width=""602"" height=""265""></p> <p> <p>DotVVM má v mnohom filozofiu presne opačnú. Snaží sa čo najviac programátora odbremeniť od písania klientskej logiky v JavaScripte aj za cenu toho,&nbsp; že pred ním implementačné detaily ukryje. DOTHTML markup ktorým sú views písané sa narozdiel od markupu Razor snaží minimalizovať C# kód v samotnom view. View je písaný v štandardnom HTML rozšírenom o sadu vlastných tagov s prefixom “dot:”. View ktoré navrhujete sa ale bude líšiť od reálne vykreslenej stránky. DotVVM zavádza totiž koncept Controls. Celé view je tvorené kontrolkami, ktoré majú každá svoju vnútornú javascriptovú logiku a definovaný spôsob, akým sa vykesľujú. Do jednoduchých kontroliek ako napríklad TextBox, ktorý vykreslí len jeden <span style=""font-family: courier new; background-color: #ddd"">&lt;input&gt;</span> tag napojený na view-model, až po komplexné ako napríkad GridView, ktorý vykreslí zo zadanej kolekcie dát z view-modelu tabuľkový prehľad s možnosťou úpravy riadkov, radenia a stránkovania.&nbsp; Dáta z view-modelu sa do kontroliek vo view jednosmerne alebo aj obojsmerne naviažu pomocou “data-bindingov” vo view. Používa sa na to syntax inšpirovaná desktopovým prezentačným frameworkom WPF. Data-bindingy podporujú jednoduché C# výrazy, ktoré uľahčujú naviazanie dát ale riadenie toku programu a zložitejšie C# konštrukcie nepodporujú.  <p>Výhodou DotVVM je, že pri požiadavke na server sa neprekresľuje celá stránka, ale naspäť sa pošle len zmena view-modelu a prípadne HTML ktoré sa zmenilo. Komunikáciu zo serverom tok dát medzi medzi kontrolkami view rieši DotVVM interne. Dáta view-modelu sú naviac dostupné ako javascriptové objekty takže napojenie vlastnej javascriptovej logiky nie je problém. Okrem využívania kontroliek, ktoré sú súčasťou samotného frameworku je samozrejme možné vytvoriť aj vlastné kontrolieky a zaregistrovať ich v rámci DotVVM, alebo ich distribuovať buď ako knižnicu DLL. To podporuje scenár, kedy skúsený team vývojárov vyvýja sadu všeobecne použiteľných kontroliek zatiaľ čo menej skúsený team ich potom využíva a zostavuje weby podľa požiadaviek zákazníka.  <p>Nevýhodou je, že na pozadí každého view je logika ktorú nie je na prv pohľad vidieť a treba s ňou rátať a porozumieť jej. Dá sa požívať aj ako ""black box"", ale náročnejšie scénare to už nestačí. Ale framework ako taký stále dokáže ušetriť veľkú časť repetitívneho klientského aj serverového kódu, obzvášť pokiaľ web obsahuje zložitejšie procesy, alebo wizardy.  <h2><font style=""font-weight: normal"">Záverom</font></h2> <p> <p>V tomto článku som predstavil základné vlastnosti ASP.NET MVC od Microsoftu a frameworku DotVVM od firmy Riganti ako dve alternatívy pre webový vývoj na .NET core. MVC sa snaží o základnú abstrakciu interakcie so serverom a návrh webu skrze priamočiare a transparentné nástroje a všetko ostatné necháva v rukách programátora.  <p>Ako alternatíva DotVVM sa snaží riešiť čo najviac z bežných úloh pri návrhu “line of business” aplikácií a tak programátorom ušetriť čas a peniaze. Naviac poskytuje dostatočnú modularitu a rozšíriteľnosť.   ')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8525, 24, CAST(N'2017-03-19T12:05:23.757' AS DateTime), N'Azure functions – “serverless architecture”', N'<p>Microsoft neustále rozšiřuje nabídku služeb své cloudové platformy Azure. Jednou z mladších jsou Functions. Pokud se podíváte na produktovou stránku Functions, uvidíte v jednom nadpisu “Zpracování událostí s architekturou kódu bez serveru”. To neznamená, že vaše funkce neběží na serverech, samozřejmě běží v datacentrech Microsoftu. Pod pojmem “""architekturou kódu bez serveru” se skrývá to, že Functions jsou relativně malé bloky kódu, které jsou izolované.</p> <p>Functions můžou být spouštěné periodicky na základě Timer Triggeru (zadávaný v cron formátu), události v jiné službě na Azure (Event Hub, Queue Storage, Service Bus), Http requestu nebo manuálně.</p> <p>Functions podporují celou řadu jazyků: JavaScript, C#, F#, Python, PHP, Bash, Batch a PowerShell</p> <h2>K čemu je to dobré? </h2> <p>Jeden z modelových příkladů je vytvoření obrázků v různých velikostech pro různé náhledy na základě uložení originálního obrázku do Blob storage. Velmi běžné je provádění různých operací na základě časovače. V jednom z našich projektů voláme každých 5 minut Machine Learning API, které nám vrací hodnoty a ty následně nastavujeme do IoT zařízení. Dalším příkladem z praxe je automatické zamykání časových výkazů. Každý den večer se spouští funkce, která volá API našeho docházkového systému a zamyká záznamy z předchozího dne.</p> <h2>Jak na to?</h2> <p>Na adrese <a href=""https://functions.azure.com"">https://functions.azure.com</a> stačí kliknout na tlačítko “Vyzkoušejte zdarma” a spustí se průvodce, který vám umožní vytvořit funkci rychle pomocí některé z předpřipravených funkcí.</p> <p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_2.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_thumb.png"" width=""846"" height=""446""></a></p> <p>Po výběru scénáře, jazyka a vytvoření funkce budete vyzvání k přihlášení se k účtu. Pro výchozí nastavení se vytvoří následující funkce otevřená v online editoru:</p> <p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_4.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_thumb_1.png"" width=""846"" height=""190""></a></p> <p>V záložce develop je k dispozici online editor, v jeho horní části se nachází tlačítka “Save” a “Save and run”. V pravém vyjížděcím menu je k dispozici správa souborů, kdy můžete přidat, smazat nebo nahrát vlastní soubor. Pro C# mají soubory příponu .csx, výchozí se jmenuje run.csx. Druhým souborem je function.json, kde jsou uloženy informace o nastavení. Další položka pravého menu je Test, která umožňuje testovat aktuální kód v případě, že přijímáte Http requesty. Poslední položka je správa klíčů.</p><pre class=""brush:csharp"" style=""height: 189px; width: 105.91%"">using System;
public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($""C# Timer trigger function executed at: {DateTime.Now}"");
}</pre>
<p>Vstupním bodem je statická metoda Run, která má dva parametry. TimerInfo, jak je z názvu patrné, obsahuje informace o čase, kdy byla funkce spuštěná. TraceWriter slouží pro logování, které je vidět v záložce monitor.</p>
<p>Pod záložkou integrate hlavního menu se skrývají možnosti nastavení triggerů, inputu a outputu funkce. V záložce manage je možné zapnout/vypnout funkci, smazat a nebo opět spravovat klíče.</p>
<p>Monitor okno může vypadat například následovně:</p>
<p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_6.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure_A0FB/image_thumb_2.png"" width=""846"" height=""460""></a></p>
<p>Nyní můžete psát funkce dle své potřeby, spouštět a monitorovat. V případě, že chcete použít nějaký nuget package, k tomu slouží direktiva r#, pro načtení dalších .csx souborů direktiva #load</p><pre class=""brush:csharp"" style=""height: 167px; width: 97.09%"">#r ""Newtonsoft.Json""
#load ""Common.csx"" 
using System;
using System.Text;
using Newtonsoft.Json;</pre>
<h2>Závěr</h2>
<p>Azure Functions jsou ideálním nástrojem pro drobnější operace, které chcete provádět na základě vnějších podmětů nebo časového spínače. Nastínili jsme si možné scénáře, ukázali jak vytvořit základní funkci a popsali webového prostředí pro functions. Příště si ukážeme scénáře, kdy funkce reaguje na změnu v blob storage nebo jak volat funkci pomocí Http requestu.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8526, 4, CAST(N'2017-03-19T14:58:37.767' AS DateTime), N'Prezentace a demo z přednášky: Logování v produkčních aplikacích', N'<p>Zde si můžete stáhnout prezentace, demo a script pro instalaci a konfiguraci infrastruktury:</p> <p><a title=""https://1drv.ms/u/s!ALlLH3SUG3U7i_9x"" href=""https://1drv.ms/u/s!ALlLH3SUG3U7i_9x"">https://1drv.ms/u/s!ALlLH3SUG3U7i_9x</a></p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8527, 24, CAST(N'2017-03-22T07:00:00.000' AS DateTime), N'Azure functions – blob storage', N'<p>V předchozím článku jsme si popsali co a k čemu Azure Functions jsou, jak je vytvořit nebo jak správa functions na portále Azure vypadá. Dnes si ukážeme, jak vytvořit funkci, která bude reagovat na změny v blob storage a nebo jejím výstupem bude nový blob.</p> <h2>Blob storage Trigger</h2> <p>Vytvoříme si novou funkci, kde je připraveno množství templatů. Vybereme “BlobTrigger-CSharp”.</p> <p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_4.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_thumb_1.png"" width=""860"" height=""250""></a></p> <p>V průvodci je potřeba zvolit Storage account connection. Já už ho mám předem připravený, pokud nemáte, jednoduše si ho vytvoříte. Do pole Path je třeba zadat cestu ke kontejneru.</p> <p>Azure vygeneruje následující kód:</p><pre class=""brush:csharp"">public static void Run(Stream myBlob, string name, TraceWriter log)
{
    log.Info($""C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes"");
    
}</pre>
<p>Já jsem v Path poli triggeru smazal “{name}”, proto musíme smazat parameter name I v metodě. Změníme kód tak, aby přečetl obsah blobu.</p><pre class=""brush:csharp"">using System;
public async static Task Run(Stream myBlob, TraceWriter log)
{
    log.Info($""C# Blob trigger function. Content of blob is:"");
    using(var reader = new StreamReader(myBlob))
    {
        log.Info(reader.ReadToEnd());
    }
}</pre>
<p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_9.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; margin: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_thumb_3.png"" width=""586"" height=""303""></a></p>
<p>Po nahrání připraveného souboru do blobu je vidět v logu tento výstup:</p><pre class=""brush:text"" style=""height: 130px; width: 99.07%"">2017-03-19T13:25:08.535 Function started (Id=96e41c5a-a8bb-4b2f-a5a4-85b45eb51a3a)
2017-03-19T13:25:08.535 C# Blob trigger function. Content of blob is:
2017-03-19T13:25:08.550 Hello world!
2017-03-19T13:25:08.550 Function completed (Success, Id=96e41c5a-a8bb-4b2f-a5a4-85b45eb51a3a)</pre>
<p>Do funkce vstupuje blob jako Stream, co když nám to ale nestačí? Functions, v tomto případě BlobTrigger, nám umožňují úpravu vstupních parametrů a jejich typů. V <a href=""https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob"">dokumentaci</a> najdete rozsáhlý popis možností, nám pro potřeby článku postačí vědět, že můžeme použít ICloudBlob. Upravíme náš kód do následující varianty:</p><pre class=""brush:csharp"" style=""height: 304px; width: 100.31%"">#r ""Microsoft.WindowsAzure.Storage""
using System;
using Microsoft.WindowsAzure.Storage.Blob;
public async static Task Run(ICloudBlob myBlob, TraceWriter log)
{
    log.Info($""C# Blob trigger function. Content of blob {myBlob.Name} is:"");
    using(var reader = new StreamReader(await myBlob.OpenReadAsync()))
    {
        log.Info(reader.ReadToEnd());
    }
}</pre>
<p>Nejdříve si nalinkujeme knihovnu Microsoft.WindowsAzure.Storage a následně přidáme using potřebného namespacu. V prvním volání log.Info() přidáme název blobu, následně přidáme kód pro vyčtení obsahu. Výpis se příliš lišit nebude, ale díky rozhraní ICloudBlob máme celou řadu nových možností jak s blobem manipulovat.</p>
<h2>Blob jako výstup funkce</h2>
<p>V této části navážeme na předchozí kód a ukážeme si, jak vytvořit nový blob jako výstup funkce. Vytvořený blob bude vracet obsah původního blobu doplněný o informace o zpracování. Nejdříve je potřeba na záložce Integrate nastavit výstup. Klikneme na tlačítko “New Output” a vybereme “Azure Blob Storage”. Opět je třeba nastavit Storage account connection a Path, do které vložím “outputblob/output.txt”. Outputblob je opět předem připravený kontejner. Pokud bychom zvolili původní kontejner “functionblob”, funkce by se volala neustále znovu, protože výstupem je nový blob, který spustí trigger a tak pořád dokola. Výstup můžeme předávat formou parametru, nebo jako návratovou hodnotu. Já zvolím návratovou hodnotu.</p><pre class=""brush:csharp"">#r ""Microsoft.WindowsAzure.Storage""
using System;
using Microsoft.WindowsAzure.Storage.Blob;

public async static Task&lt;string&gt; Run(ICloudBlob myBlob, TraceWriter log)
{
    log.Info($""The following blob has triggered the function: {myBlob.Name}"");
    log.Info(""Content of the blob is:"");
    using(var reader = new StreamReader(await myBlob.OpenReadAsync()))
    {
        var content = reader.ReadToEnd();
        log.Info(content);
        return content + Environment.NewLine + $""{myBlob.Name} processed by function at {DateTime.Now}"";
    }
}</pre>
<p>V nastavení outputu jsme uvedli cestu včetně názvu, proto můžeme vrátít string, který se zapíše do výstupního blobu. Po opětovném nahrání souboru blob.txt do functionblob kontejneru se funkce spustí, a do kontejneru outputblob vloží output.txt s očekávaným obsahem.</p>
<p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_11.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/Azure-functions--_BA93/image_thumb_4.png"" width=""848"" height=""159""></a></p><pre class=""brush:text"">Hello world!
blob.txt processed by function at 3/19/2017 2:09:49 PM</pre>
<p>Podobně jako output je možné nastavit také input, který vstupuje jako parametr do metody Run. Inputů a outputů je možné předávat více. A v čem se liší input od triggeru? Můžete například vytvořit funkci, kterou chcete spouštět periodicky, ale potřebujete na vstupu mít blob. V tomto případě můžete použít kombinaci TimerTriggeru a BlobStorage jako input.</p>
<h2>Závěr</h2>
<p>V tomto článku jsme si ukázali práci s bloby. V první části jsme reagovali na změnu v kontejneru a vypsali obsah nového blobu. V druhé části jsme vzali obsahu načteného blobu, přidali informace o zpracování a poslali na výstup jako nový blob v jiném kontejneru. Příště si ukážeme, jak volat funkci pomocí Http requestu.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8529, 15, CAST(N'2017-04-14T16:54:10.040' AS DateTime), N'Automatické verzování AssemblyVersion a AssemblyFileVersion v .NET Core (csproj)', N'<p>V .NET Core projektu (v novém formátu csproj) již nelze v <strong>AssemblyVersion</strong> atributu použít hvězdičku (*) (např: AssemblyVersion(""0.9.*"")) pro automatické generování <strong>AssemblyVersion</strong> a <strong>AssemblyFileVersion</strong> - <em>Chyba CS7034: The specified version string does not conform to the required format - major[.minor[.build[.revision]]]. </em>Navíc atributy se nyní místo do souboru <strong>AssemblyInfo.cs</strong> ukládají primárně přímo do souboru .csproj jako MSBuild vlastnosti (MSBuild pak na pozadí soubor AssemblyInfo.cs generuje).</p> <p>Hledal jsem způsob, jak číslo verze opět automaticky generovat (například proto, že číslo verze se aplikací zapisuje do různých trace a error interních logů). K tomuto tématu jsem našel tyto dva příspěvky na <em>stackoverflow.com</em>:<br><a title=""http://stackoverflow.com/questions/43274254/setting-the-version-number-for-net-core-projects-csproj-not-json-projects/43280282#43280282"" href=""http://stackoverflow.com/questions/43274254/setting-the-version-number-for-net-core-projects-csproj-not-json-projects/43280282#43280282"" target=""_blank"">http://stackoverflow.com/questions/43274254/setting-the-version-number-for-net-core-projects-csproj-not-json-projects/43280282#43280282</a><br><a title=""http://stackoverflow.com/questions/43019832/auto-versioning-in-visual-studio-2017-net-core/43194610#43194610"" href=""http://stackoverflow.com/questions/43019832/auto-versioning-in-visual-studio-2017-net-core/43194610#43194610"" target=""_blank"">http://stackoverflow.com/questions/43019832/auto-versioning-in-visual-studio-2017-net-core/43194610#43194610</a><br>Bohužel ani v jednom z nich jsem ale nenašel řešení, které by ně uspokojilo, protože potřebuji číslo verze generovat <strong>nejen při vytváření automatizovaného buildu</strong> na Build serveru, ale například i při ručním buildu nebo <strong>Publish z Visual Studia</strong>.</p> <p>Neúspěšně jsem si prošel pokusem volat vlastní .NET Core tool, který mění soubory .csproj. Toto nefunguje, protože pří buildu je soubor .csproj hned nahrán do paměti a jakákoliv pozdější jeho modifikace se již neprojeví. Po zhruba dvou dnech jsem došel k následujícímu řešení.</p> <p>Hodnoty pro <strong>AssemblyFileVersion</strong> a <strong>AssemblyVersion</strong> budeme mít zadány přímo v projektovém <strong>.csproj</strong> souboru a <strong>nebudeme pro ně používat <em>AssemblyInfo.cs</em></strong> – v souboru .csproj je to MSBuild vlastnost <strong>FileVersion</strong> (z ní MSBuild generuje <em>AssemblyFileVersionAttribute</em>) a vlastnost <strong>AssemblyVersion</strong> (generuje <em>AssemblyVersionAttribute</em>). Pak <strong>v rámci MSBuild procesu</strong> ke změně čísla verze nebudeme měnit žádné soubory, ale pouze <strong>přenastavíme hodnoty těchto vlastností</strong> <strong>FileVersion</strong> a <strong>AssemblyVersion</strong> (Případně můžeme změnit jen některou z těchto vlastností, podle potřeby).</p> <h3>Vlastní MSBuild task</h3> <p>Vytvoříme vlastní <strong>MSBuild task</strong>, který bude pouze vracet generované číslo verze. Ke generování používá stejný algoritmus, jako bylo chování hvězdičky (*) v AssemblyVersion tj. počet dnů od 1.1.2000 (jako build číslo) a půlka počtu sekund od půlnoci (jako revision číslo).</p> <p>Zdrojový kód MSBuild tasku je následující:</p><pre class=""brush: csharp; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">public class GetCurrentBuildVersion : Task
{
    [Output]
    public string Version { get; set; }

    public string BaseVersion { get; set; }

    public override bool Execute()
    {
        var originalVersion = System.Version.Parse(this.BaseVersion ?? ""1.0.0"");

        this.Version = GetCurrentBuildVersionString(originalVersion);

        return true;
    }

    private static string GetCurrentBuildVersionString(Version baseVersion)
    {
        DateTime d = DateTime.Now;
        return new Version(baseVersion.Major, baseVersion.Minor,
            (DateTime.Today - new DateTime(2000, 1, 1)).Days,
            ((int)new TimeSpan(d.Hour, d.Minute, d.Second).TotalSeconds) / 2).ToString();
    }
}</pre>
<p>Task je ve třídě <strong>GetCurrentBuildVersion</strong>, která dědí ze třídy <strong>Microsoft.Build.Utilities.Task</strong> (z <strong>NuGet balíčku Microsoft.Build.Utilities.Core</strong>). Vstupem tasku je vlastnost <strong>BaseVersion</strong> (nepovinná), ze které se odvodí Major a Minor čísla verze. Výstupem pak je plné číslo verze ve vlastnosti <strong>Version</strong>.</p>
<p>Třídu umístíme například do <strong>.NET Standard 1.3 class library</strong> (ale můžeme použít i jiný druh projektu, který se kompiluje do dll). .csproj soubor vypadá takto:</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">&lt;Project Sdk=""Microsoft.NET.Sdk""&gt;
  &lt;PropertyGroup&gt;
    &lt;TargetFramework&gt;netstandard1.3&lt;/TargetFramework&gt;
    &lt;AssemblyName&gt;DC.Build.Tasks&lt;/AssemblyName&gt;
    &lt;RootNamespace&gt;DC.Build.Tasks&lt;/RootNamespace&gt;
    &lt;PackageId&gt;DC.Build.Tasks&lt;/PackageId&gt;
    &lt;Product&gt;DC.Build.Tasks&lt;/Product&gt;
    &lt;AssemblyTitle&gt;DC.Build.Tasks&lt;/AssemblyTitle&gt;
    &lt;Description&gt;MSBuild Tasks&lt;/Description&gt;
  &lt;/PropertyGroup&gt;

  &lt;ItemGroup&gt;
    &lt;PackageReference Include=""Microsoft.Build.Framework"" Version=""15.1.1012"" /&gt;
    &lt;PackageReference Include=""Microsoft.Build.Utilities.Core"" Version=""15.1.1012"" /&gt;
  &lt;/ItemGroup&gt;

&lt;/Project&gt;</pre>

<p>Pokud nechcete tuto MSBuild Task library vytvářet sami, můžete použít mojí z <strong>GitHubu </strong><a title=""holajan/DC.Build.Tasks"" href=""https://github.com/holajan/DC.Build.Tasks"" target=""_blank""><strong>holajan/DC.Build.Tasks</strong></a>.</p>
<h3>Použití MSBuild tasku pro nastavení FileVersion a AssemblyVersion</h3>
<p>Vytvoření MSBuild tasku byla ta jednodušší část, teď zbývá nastavit jeho <strong>použití v MSBuild project .csproj souboru</strong>. Nejprve pomoci <strong>UsingTask</strong> importujeme náš task GetCurrentBuildVersion. Následující definice předpokládá, že dll s GetCurrentBuildVersion taskem (v mém případě <em>DC.Build.Tasks.dll</em>) leží o jeden nadřazený adresář víš než editovaný .csproj soubor:</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">&lt;UsingTask TaskName=""GetCurrentBuildVersion"" AssemblyFile=""$(MSBuildThisFileFullPath)\..\..\DC.Build.Tasks.dll"" /&gt;
</pre>
<p>Dále potřebujeme zavést <strong>Target</strong> sekci, která se bude volat před buildem. Použijme k tomu nastavení <strong>BeforeTargets</strong> a napojíme se na&nbsp; target <strong>BeforeBuild</strong>.</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">&lt;Target Name=""BeforeBuildActionsProject1"" BeforeTargets=""BeforeBuild""&gt;</pre>
<p>Důležité je zde pojmenování target sekce, jméno může být libovolné (v příkladu <em>BeforeBuildActionsProject1</em>), ale musí být v každém projektu různé. Pokud tedy budeme v solution mít více projektů, ve kterých budeme chtít generovat číslo verze, v každém z nich tuto sekci pojmenujeme jinak, aby se při build procesu nevolala vícekrát stejná sekce.</p>
<p>V tomto target budeme pak volat task <strong>GetCurrentBuildVersion</strong> a vrácenou hodnotou (z vlastnosti tasku Version) přenastavíme číslo verze v MSBuild property <strong>FileVersion</strong>. Poté ještě tu samou hodnotu nastavíme i do property <strong>AssemblyVersion</strong> (pokud potřebujete automaticky generovat například pouze FileVersion, můžete toto nastavení vynechat).</p>
<p>Celé to pak <strong>v projektovém souboru .csproj</strong> vypadá takto:</p><pre class=""brush: xml; auto-links: true; collapse: false; first-line: 1; gutter: false; html-script: false; light: false; ruler: false; smart-tabs: false; tab-size: 4; toolbar: false;"">&lt;Project Sdk=""Microsoft.NET.Sdk""&gt;
  &lt;UsingTask TaskName=""GetCurrentBuildVersion"" AssemblyFile=""$(MSBuildThisFileFullPath)\..\..\DC.Build.Tasks.dll"" /&gt;

  &lt;PropertyGroup&gt;
    ...
    &lt;AssemblyVersion&gt;0.9.0.0&lt;/AssemblyVersion&gt;
    &lt;FileVersion&gt;0.9.0.0&lt;/FileVersion&gt;
  &lt;/PropertyGroup&gt;

  ...

  &lt;Target Name=""BeforeBuildActionsProject1"" BeforeTargets=""BeforeBuild""&gt;
    &lt;GetCurrentBuildVersion BaseVersion=""$(FileVersion)""&gt;
      &lt;Output TaskParameter=""Version"" PropertyName=""FileVersion"" /&gt;
    &lt;/GetCurrentBuildVersion&gt;
    &lt;PropertyGroup&gt;
      &lt;AssemblyVersion&gt;$(FileVersion)&lt;/AssemblyVersion&gt;
    &lt;/PropertyGroup&gt;
  &lt;/Target&gt;

&lt;/Project&gt;
</pre>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8530, 24, CAST(N'2017-04-18T06:00:00.000' AS DateTime), N'Azure functions–http trigger', N'<p>V minulých dílech jsme si popsali základy Azure Functions, jak funkci vytvořit nebo třeba jak pracovat s bloby jako vstupními či výstupními parametry. V tomto článku se podíváme na to, jak volat funkcí pomocí http požadavku.</p> <h1>http trigger</h1> <p>Microsoft nedávno přepracoval uživatelské rozhraní pro správu functions, ale nic zásadního se nezměnilo. Pro tvorbu funkce s http triggerem zle využít předpřipravenou šablonu jako u celé řady dalších případů. Vybereme HttpTrigger-CSharp, zobrazí se formulář pro vyplnění detailů funkce. Za povšimnutí stojí authorization level, který určuje, kdo může funkci volat. V případě “function” je potřeba při volání přiložit function key, v případě “admin” je vyžadován master key. Pro jednoduchost vybereme možnost “anonymous”, kdy se není třeba pro volání nijak autorizovat.</p> <p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_2.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_thumb.png"" width=""466"" height=""447""></a></p> <p>Je vygenerován následující kód. Metoda Run přijímá parametr typu <a href=""https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessage(v=vs.118).aspx"">HttpRequestMessage</a>. </p><pre class=""brush:csharp"" style=""height: 546px; width: 97.69%"">using System.Net;

public static async Task&lt;HttpResponseMessage&gt; Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info(""C# HTTP trigger function processed a request."");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q =&gt; string.Compare(q.Key, ""name"", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync&lt;object&gt;();

    // Set name to query string or body data
    name = name ?? data?.name;

    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, ""Please pass a name on the query string or in the request body"")
        : req.CreateResponse(HttpStatusCode.OK, ""Hello "" + name);
}</pre>
<p>Funkce se snaží přečíst parameter “name” předaný v URL nebo těle požadavku a vrátí pozdrav nebo upozornění, že jméno nebylo vyplněno. Pro testování funkce může posloužit panel “Test”, který je součástí editoru.</p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_4.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_thumb_1.png"" width=""623"" height=""853""></a> 
<p>My si z funkce uděláme jednoduché API pro kalkulačku. Nejprve je potřeba upravit několik nastavení na záložce “Integrate”, konkrétně náš http trigger. V prvním kroku ponecháme jedinou povolenou http metodu pouze GET. V druhém kroku upravíme “Route template” na hodnotu “calculate/add/{numberOne}/{numberTwo}”. Tím upravíme URL, na které je funkce dostupná a zároveň pomocí složených závorek řekneme, že očekáváme dva parametry.</p>
<p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_6.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_thumb_2.png"" width=""967"" height=""277""></a></p>
<p>Jakmile se vrátíme do editoru funkce, všimněte si, že v otevřeném test panelu přibyly oba dva paremetry – numberOne a numberTwo. Nyní přepíšeme funkci tak, aby sčítala dva parametry.</p><pre class=""brush:csharp"" style=""height: 623px; width: 99.16%"">using System.Net;

public static HttpResponseMessage Run(HttpRequestMessage req, int numberOne, int numberTwo, TraceWriter log)
{
    log.Info(""C# HTTP trigger function processed a request."");
    log.Info(""Input parameters"");
    log.Info($""Number one: {numberOne}"");
    log.Info($""Number two: {numberTwo}"");

    var result = new ResultStruct
    {
        NumberOne = numberOne,
        NumberTwo = numberTwo,
        Result =numberOne + numberTwo 
    };
    return req.CreateResponse(HttpStatusCode.OK, result);
    
}

struct ResultStruct
{
    public int NumberOne {get; set;}
    public int NumberTwo{get; set;}
    public int Result{get; set;}
}</pre>
<p>Protože v metodě Run nezůstane v našem případě žádné asynchronní volání, tak změníme návratový typ na HttpResponseMessage a odebereme klíčové slovo async. Rozšíříme vstupní parametry o int numberOne, respektive int numberTwo. Functions nám parametry naplní na základě volání funkce. Následný kód je velice jednoduchý. Nejdříve zalogujeme vstupní parametry, pak naplníme strukturu, kterou pošleme na výstup. Výsledkem bude následující json.</p><pre class=""brush:js"">{
    ""NumberOne"": 5,
    ""NumberTwo"": 2,
    ""Result"": 7
}</pre>
<p>Když máme funkci vyzkoušenou, tak ještě přidáme jednoduchou autorizaci. Na záložce Integrate změníme Authorization level na “Function”. Samotná autorizace je jednoduchá, stačí do URL přidat parameter code. Klíč je k dispozici na záložce manage po kliknutí na odkaz Click to show. Klíčů je možno vytvořit víc, nezávisle je na sobě odebírat atd.</p>
<p>Funkci vyzkoušime volat z REST konzole v prohlížeči (libovolný plugin poslouží, já používám RestMan pro Operu). URL adresu funkce získáme velice jednoduše, stačí v pravém horním rohu editoru kliknout na odkaz Get function URL, který vrátí URL včetně vloženého klíče. URL stačí zkopírovat do konzole. Pokud smažeme parametr code, celkem očekávaně funkce vrátí kód 401, unauthorized. Po vrácení klíče na své místo se vrátí validní výsledek. Druhou alternativou pro autorizaci je vytvoření hlavičky “x-functions-key” obsahující klíč. Výsledek pak může vypadat přibližně takto:</p>
<p><a href=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_8.png""><img title=""image"" style=""border-left-width: 0px; border-right-width: 0px; background-image: none; border-bottom-width: 0px; padding-top: 0px; padding-left: 0px; display: inline; padding-right: 0px; border-top-width: 0px"" border=""0"" alt=""image"" src=""https://dotnetportalprod.blob.core.windows.net/files/Windows-Live-Writer/9bf61697ef20_8B82/image_thumb_3.png"" width=""799"" height=""633""></a></p>
<h1>Závěr</h1>
<p>Tentokrát jsme si ukázali, jak vytvořit funkci, která je k dispozici pomocí http volání a může sloužit jako část našeho API. Vytvořili jsme velice jednoduchou funkci, která slouží k demonstraci možností předávání vstupních parametrů pomocí url, generování výstupu ve formátu json a autorizaci pomocí klíče. Příště se zaměříme na to, jak zapojit functions do DevOps.</p>')
INSERT [dbo].[Articles] ([Id], [BlogId], [PublishedDate], [Title], [Html]) VALUES (8531, 3, CAST(N'2017-04-17T17:51:26.017' AS DateTime), N'Jak funguje return false v JavaScriptu', N'<p>Jeden z uživatelů <a href=""https://www.dotvvm.com""><strong>DotVVM</strong></a> nám včera hlásil zajímavou chybu – pokud v komponentě <a href=""https://www.dotvvm.com/docs/controls/builtin/GridView/1-1"">GridView</a> do některé z buněk umístíte tlačítko, a zároveň na řádky pověsíte <em>RowDecorator</em>, který na nich odchytává událost <em>Click</em>, při kliknutí na tlačítko se vyvolají obě dvě.</p> <p>DotVVM totiž při použití tlačítka renderuje něco jako tohle: </p><pre class=""brush:html"">&lt;button onclick=""dotvvm.postBack(…);return false;"" /&gt;</pre>
<p>Oprava byla velmi jednoduchá, ale přivedlo nás to k zajímavé otázce – jak vlastně to <strong>return false </strong>u event handlerů funguje? Chvilka hledání ukázala, že je v tom, jak už to ve světě Javascriptu bývá, slušný zmatek.</p>
<p>&nbsp;</p>
<p>Každá událost, která v DOM nastane, vytvoří některý z <a href=""https://developer.mozilla.org/en-US/docs/Web/API/Event"">Event objektů</a>, kde o daném eventu najdete detaily. Ve všech prohlížečích kromě Firefoxu je tento objekt zpřístupněn pod <strong>window.event</strong>, a kromě toho je předán jako první parametr do funkce sloužící jako event handler. V HTML atributech (onclick) atd je dostupná jako proměnná <strong>event</strong>. </p>
<p>Takže pokud zaregistruji handler pomocí <strong>addEventListener</strong>, v první parametru dostanu patřičný event objekt. Akorát ve starých IE a bůhví kde ještě tohle přes <strong>attachEvent</strong> nefungovalo, takže se v různých frameworcích ustálila konvence dělat to nějak takto:</p><pre class=""brush:js"">document.getElementById(""button"").addEventListener(""click"", myhandler);
function myhandler(e) {
  e = e || window.event;
  
  // ...
}</pre>
<p>Pokud by náhodou do event handleru event objekt nebyl předán, dohledá se ve window.event, a budeme spoléhat na to, že browser bude mít implementováno jedno, nebo druhé.</p>
<p>&nbsp;</p>
<p>Na event objektu pak můžete volat mimo jiné následující dvě funkce:</p>
<ul>
<li><strong>preventDefault()</strong> – tato funkce zruší výchozí chování komponenty (např. přechod na adresu uvedenou v <strong>href </strong>atributu u odkazu nebo submitování formuláře). 
<li><strong>stopPropagation()</strong> – tato funkce zamezí propagaci této události do rodičovských elementů (standardně když tlačítko s onclick umístíte dovnitř divu, který bude mít také onclick, zavolají se oba – událost bublá směrem nahoru.</li></ul>
<p>&nbsp;</p>
<p>Pokud v handleru použijete <strong>return false</strong>, chová se to dost nekonzistentně:</p>
<ul>
<li>Pokud <strong>return false</strong> uvedete například v atributu <strong>onclick</strong>, zavolá se <strong>preventDefault()</strong>, ale <strong>stopPropagation()</strong> už ne. 
<li>Pokud <strong>return false </strong>uvedete v event handleru navázeném přes <strong>addEventListener</strong>, nedělá to vůbec nic – defaultní chování se nepotlačí a událost bublá. 
<li>Pokud <strong>return false </strong>uvedete v event handleru navázaném pomocí starého <strong>attachEvent </strong>(doporučuji nepoužívat), zavolá se pouze <strong>preventDefault()</strong>. 
<li>Pokud <strong>return false </strong>použijete v event handleru navázaném pomocí jQuery, zavolá to jak <strong>preventDefault()</strong> tak <strong>stopPropagation()</strong>. </li></ul>')
SET IDENTITY_INSERT [dbo].[Articles] OFF
SET IDENTITY_INSERT [dbo].[Tags] ON 

INSERT [dbo].[Tags] ([Id], [Name]) VALUES (1, N'C#')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (2, N'VB.NET')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (3, N'ASP.NET WebForms')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (4, N'ASP.NET MVC')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (5, N'XNA')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (6, N'DirectX')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (7, N'SQL')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (8, N'Entity Framework')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (9, N'LINQ')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (10, N'Visual Studio')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (11, N'WPF')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (12, N'Silverlight')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (13, N'Windows Phone')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (14, N'ADO.NET')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (15, N'Azure')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (16, N'WCF/WS')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (17, N'Testování')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (18, N'Offtopic')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (19, N'Hardware')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (20, N'Komponenty')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (21, N'Architektura')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (22, N'Algoritmy')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (23, N'Optimalizace')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (24, N'XML')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (25, N'Threading')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (26, N'WinForms')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (27, N'ASP.NET/IIS')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (28, N'HTTP/HTML')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (29, N'Reflexe')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (30, N'Databáze')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (31, N'I/O operace')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (32, N'VB6/VBA')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (33, N'Tisk')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (34, N'C++/C')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (35, N'Grafika')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (36, N'Bezpečnost')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (37, N'Compact Framework')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (38, N'Microframework')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (39, N'Regulární výrazy')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (40, N'Office')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (41, N'Powershell')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (42, N'WinAPI')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (43, N'.NET')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (44, N'JavaScript')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (45, N'.NET Tips')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (46, N'Internet Explorer')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (50, N'Workflow Foundation')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (51, N'WIF')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (52, N'Windows Users Group')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (53, N'TFS')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (54, N'IT')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (56, N'PostSharp a AOP')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (57, N'Debugging')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (58, N'Knihovny')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (59, N'Internals')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (60, N'Runtime (CLR)')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (61, N'Kompilátor')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (62, N'F#')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (63, N'Reklamní sdělení')
INSERT [dbo].[Tags] ([Id], [Name]) VALUES (64, N'Xamarin')
SET IDENTITY_INSERT [dbo].[Tags] OFF
SET IDENTITY_INSERT [dbo].[ArticleTags] ON 

INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (1, 8443, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (2, 8445, 15)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (3, 8445, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (4, 8447, 10)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (5, 8449, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (6, 8449, 5)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (7, 8450, 10)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (8, 8450, 15)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (9, 8457, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (10, 8457, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (11, 8457, 29)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (12, 8457, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (13, 8477, 28)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (14, 8477, 46)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (15, 8493, 10)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (16, 8493, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (17, 8494, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (18, 8494, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (19, 8494, 28)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (20, 8494, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (21, 8494, 44)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (22, 8498, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (23, 8499, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (24, 8499, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (25, 8504, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (26, 8504, 44)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (27, 8509, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (28, 8531, 28)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (29, 8531, 44)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (30, 8526, 21)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (31, 8526, 36)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (32, 8526, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (33, 8526, 54)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (34, 8446, 10)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (35, 8448, 36)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (36, 8448, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (37, 8448, 51)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (38, 8459, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (39, 8459, 57)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (40, 8460, 8)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (41, 8460, 9)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (42, 8460, 25)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (43, 8462, 10)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (44, 8462, 53)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (45, 8462, 61)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (46, 8464, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (47, 8464, 36)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (48, 8485, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (49, 8486, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (50, 8511, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (51, 8511, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (52, 8511, 57)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (53, 8511, 61)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (54, 6393, 56)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (55, 6395, 56)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (56, 6408, 56)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (57, 6415, 9)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (58, 8451, 17)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (59, 8451, 62)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (60, 7435, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (61, 7435, 5)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (62, 7435, 35)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (63, 7435, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (64, 8436, 1)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (65, 8436, 5)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (66, 8436, 35)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (67, 8436, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (68, 8520, 27)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (69, 8520, 28)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (70, 8520, 43)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (71, 8520, 44)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (72, 8525, 15)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (73, 8527, 15)
INSERT [dbo].[ArticleTags] ([Id], [ArticleId], [TagId]) VALUES (74, 8530, 15)
SET IDENTITY_INSERT [dbo].[ArticleTags] OFF
SET IDENTITY_INSERT [dbo].[Comments] ON 

INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (1, 6393, N'<p>Celé mi to připadá jako zbytečně komplikované přesunutí kódu z místa A na místo B a ještě za použití nějakých knihoven třetích stran...</p><p></p><p>Pokud je někdo schopen stvořit něco jako public IEnumerable&lt;IGrouping&lt;TKey, T&gt;&gt; GroupWhile(IEnumerable&lt;T&gt; source, Func&lt;T, T, bool&gt; predicate, Func&lt;T, T, TKey&gt; keyOfGroup), potom je třeba takového člověka vůbec nepouštět k psaní kódu a ne hledat způsoby, jak řešit jeho prasárny.</p>', CAST(N'2014-07-09T13:19:50.620' AS DateTime), N'77.48.126.106')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (2, 6395, N'<p>Stále mě to nepřesvědčilo o tom, že to není jen zcela zbytečná hovadina na první pohled usnadňující práci, ovšem při bližším pohledu značně omezující.</p><p></p><p>- Buď to zde není zmíněno, nebo jsem to přehlédl: Podporuje to Visual Basic .NET kód?</p><p></p><p>- Z toho co jsem pochopil, to zaznamenává neošetřené vyjímky. Pokud ano, ošetřenou vyjímku, kterou potřebuju z nějakého důvodu zaznamenat to nezaznamená?</p>', CAST(N'2014-07-19T12:59:16.350' AS DateTime), N'77.48.126.106')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (3, 6415, N'<p>1) Sekce o lamda výrazech je pěkně ošizená, takto čtenář dostává pocit, že lambda = zkrácení zápisu anonynomní metody, nic víc, nic míň. Bude nějaký rozšiřující článek?</p><p></p><p>2) Není to ani &quot;Extensions method&quot;, ani &quot;Extensions methods&quot;, ale <em>extension methods</em>.</p><p></p><p>3) &quot;dovolují přidat metodu k existujcímu datovému typu a tím ho rozšířit&quot; - to je velká lež! Extension metody nikam nic nepřidávají, pouze rozšiřují schopnosti datového typu a to, že mohou být zavolány na instanci daného typu je práce kompilátoru. Pokračuji v dalším bodě.</p><p></p><p>4) &quot;Na pozadí fungují tak&quot; - to vůbec není pozadí, ale informace, kterou každý, kdo chce takovou metodu napsat musí vědět. O tom, jak extension metody fungují na pozadí nemáš v článku ani řádek. Bude něco v příštích dílech? Něco málo o <em>overload resolution</em>, <em>method invocation</em> extension metod atd. by se hodilo. Jak je to podané zde, vypadá to jako kouzlo, než jako logika kompilátoru. Vše potřebné najdeš v specifikaci, sekce 7.6.4 a 7.6.5.</p><p></p><p>5) &quot;Implicitní datový typ&quot; - žádné takové rozlišení v .NET neexistuje. To, co máš na mysli se správně nazývá &quot;implicitně typovaná lokální proměnná&quot;. Tohle je také blbost &quot;implicitně definované typy&quot;. Datové typy v .NET musejí být definované jednoznačně, nicméně &quot;implicitně definované typy lokálních proměnných&quot;, to už je jiná.</p><p></p><p>6) &quot;Dalším významným rozšířením je yield return&quot; - už jsem to psal Filipovi, yiled return není žádné rozšíření. Jsou to pouhá dvě klíčová slova, která ve spojení dávají smysl v kontextu iterátorů, což je také oficiální název, který se pro to používá.</p>', CAST(N'2014-09-14T11:20:57.327' AS DateTime), N'89.102.136.40')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (4, 7435, N'<p>Ahoj, pěkný a srozumitelný úvod. Měl bych možná námět na jeden díl (možná díl nula nebo jen tak mimo). Vždy když se mluví o frameworcích a toolech na vývoj her, každý se hned ptá (a já vlastně také), jak je takový nástroj mocný, co se v něm dá vytvořit a kolik to dá úsilí. Zajímalo by mě, jak je to například s grafikou, zda je to spíše práce pro grafika nebo si zvládne nějakou pěknou grafiku pro hry udělat i vývojář odkojený na běžných aplikacích :)</p>', CAST(N'2015-01-14T21:46:12.853' AS DateTime), N'62.245.75.8')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (5, 7435, N'<p>Pěkné. Jen pokud těch dílů bude opravdu víc, tak by to chtělo na konci každého článku odkaz ke stažení hotového projektu.</p>', CAST(N'2015-01-15T07:20:56.230' AS DateTime), N'195.128.222.209')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (6, 8443, N'<p>Optional parametr typu String s CallerMemberName atributem?</p>', CAST(N'2015-03-22T21:52:50.900' AS DateTime), N'217.30.70.204')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (7, 8443, N'<pre class=""brush: csharp"">&#10;public string GetValue([CallerMemberName] string callerMemberName = null)&#10;{&#10;    if (String.IsNullOrEmpty(callerMemberName))&#10;        return null;            &#10;&#10;    return appSettingsCollection[callerMemberName];&#10;}&#10;</pre>', CAST(N'2015-03-23T11:56:29.157' AS DateTime), N'90.178.162.64')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (8, 8443, N'<p>Mohlo by to být takto:</p><pre class=""brush: csharp"">&#10;public class ApplicationSettings&#10;{&#10;    private readonly NameValueCollection appSettingsCollection;&#10;&#10;    public ApplicationSettings(NameValueCollection appSettingsCollection)&#10;    {&#10;        this.appSettingsCollection = appSettingsCollection;&#10;    }&#10;&#10;    public string GetValue()&#10;    {&#10;        StackTrace stackTrace = new StackTrace();&#10;        StackFrame[] stackFrames = stackTrace.GetFrames();&#10;&#10;        StackFrame callingFrame = stackFrames[1];&#10;        MethodBase method = callingFrame.GetMethod();&#10;        string fullMethodName = method.Name;&#10;        string methodName = fullMethodName.Replace(&quot;get_&quot;, &quot;&quot;).Replace(&quot;set_&quot;, &quot;&quot;);&#10;&#10;        return appSettingsCollection[methodName];&#10;    }&#10;&#10;    public string Domain&#10;    {&#10;        get&#10;        {&#10;            return this.GetValue();&#10;        }&#10;    }&#10;    public string Port&#10;    {&#10;        get&#10;        {&#10;            return this.GetValue();&#10;        }&#10;    }&#10;    public string Name&#10;    {&#10;        get&#10;        {&#10;            return this.GetValue();&#10;        }&#10;    }&#10;}&#10;</pre><p></p><p></p><p>Metoda GetValue by se dala vylepšit ještě získáním typu z property a automatickým přetypováním. V každém případě, buď jak buď, jiné řešení mě v rychlosti nenapadá a kdokoli by mi tohle napsal do kódu, tak ho exemplárně popravím před celou firmou :-D</p>', CAST(N'2015-03-24T13:31:38.657' AS DateTime), N'194.108.204.148')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (9, 8446, N'<p>Tady je ještě návod jak nastavit NuGet Feed i bez Visual Studia:</p><p><a href=""https://github.com/aspnet/Home/wiki/Configuring-the-feed-used-by-dnu-to-restore-packages"" rel=""nofollow"">https://github.com/aspnet/Home/wiki/Conf...</a></p>', CAST(N'2015-04-06T20:21:29.007' AS DateTime), N'62.168.5.14')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (10, 8451, N'<p>Framework hezký, ale ten F# je hroznej. Zkusím to o víkendu rozchodit v C# a zabalit do unit test projektu, aby se to dalo používat nějak rozumně.</p>', CAST(N'2015-04-30T14:51:28.480' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (11, 8451, N'<p>Je to pěkný, ještě by mě zajímalo, jak udělat, aby to v případě chyby předalo fail do Unit Testu ve VS aby to mohlo být součástí Buildu. Do UnitTestu jsem to dostal, ale vždy to Passne, i když je tam chyba. Neřešili jste to náhodou?</p>', CAST(N'2015-06-11T12:25:11.317' AS DateTime), N'62.24.65.165')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (12, 8457, N'<p>To je zajímavé, že v coreclr mscorlib ta metoda Assembly.Load(byte[]) je. <a href=""https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Reflection/Assembly.cs#L392"" rel=""nofollow"">https://github.com/dotnet/coreclr/blob/m...</a>. A GetExecutingAssembly je tam taky.</p>', CAST(N'2015-05-17T12:08:00.013' AS DateTime), N'217.30.72.69')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (13, 8457, N'<p>Resharper už to umí řešit. <a href=""http://i.snag.gy/soNzT.jpg"" rel=""nofollow"">http://i.snag.gy/soNzT.jpg</a></p>', CAST(N'2015-05-31T14:43:19.213' AS DateTime), N'81.161.64.26')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (14, 8457, N'<p>Nechci pranýřovat, ale spousta těch anglických výrazů má český překlad. Sestavení, aplikační domény, vlastnosti, metody rozšíření apod.</p>', CAST(N'2015-07-15T23:27:39.570' AS DateTime), N'217.168.216.135')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (15, 8460, N'<p>Divím se, že to nikdo ještě neřešil.</p>', CAST(N'2015-07-13T18:02:22.500' AS DateTime), N'62.168.5.14')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (16, 8460, N'<p>Snad už nikdo neprogramuje v češtině, a ta je tak v kódu jen jen pro ilustraci. :-)</p>', CAST(N'2015-07-14T07:40:28.407' AS DateTime), N'194.228.40.180')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (17, 8468, N'<p>To zní dobře. </p><p>Už se těším na další články</p>', CAST(N'2015-09-22T01:42:03.993' AS DateTime), N'88.103.3.127')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (18, 8491, N'<p>V lokální debug verzi IAP testovat jdou, aplikace ale musí být asociovaná se Storem, nebo dokonce musí projít certifikací první submission. Nevím s určitostí jak to přesně je, ale IAP mi fungují i v případě, kdy je aplikace nasazená z Visual Studia.</p>', CAST(N'2016-02-17T09:07:22.217' AS DateTime), N'37.48.51.201')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (19, 8493, N'<p>Pěkné.</p><p>předpokládám, že VS je vaše primární IDE, máte zkušenost i s jinými? Dokázal byste je nějak objektivně porovnat? Jak moc jdou ostatní IDE dopředu? (přijde mi, že VS mílovými kroky)</p>', CAST(N'2016-03-30T23:44:32.200' AS DateTime), N'94.199.40.193')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (20, 8493, N'<p>V update 2, také přibyla nová klávesová zkratka ctrl + alt +., takže na české klávesnici pak nelze napsat &gt; pomocí pravý alt + ., samozřejmě ji lze odpmapovat v Options -&gt; keyboard.</p>', CAST(N'2016-04-09T00:26:14.947' AS DateTime), N'81.161.64.26')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (21, 8494, N'<p>Držím vám palce !</p><p>Docela si to dáváte...</p>', CAST(N'2016-04-23T21:38:12.953' AS DateTime), N'88.103.3.127')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (22, 8494, N'<p>Uplynul skoro další rok. Co nového okolo DotVVM a kam jste postoupili?</p>', CAST(N'2017-02-07T14:00:09.787' AS DateTime), N'193.111.133.143')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (23, 8496, N'<p>Hezky napsané, přidám pár svých postřehů.</p><p></p><p>Rychlost:</p><p></p><p>S rychlostí mám též občas problémy a ne jednou to dopadne tak, že je lepší napsat si čístý SQL nebo storku a tu pak přes ExecuteQuery zavolat.</p><p></p><p>Prorůstání IQueryable</p><p></p><p>S tím mám taky problém. Částečně ho může řešit vhodná architektura (nějaký object-query pattern), kdy každý dotaz se zabalí do třídy a vlastně ven se vůbec IQueryable nebude publikovat. Problém je, že některé komponenty a prvky (webapi) queryable vyžadují na vstupu. (Třeba kendo grid) a zatím se mi nepodařilo tuto vrstvu skrýt. Ale v jedné starší aplikaci sme narazili na přesně to samé. A nakonec jsme skončili na tom, že jsme si udělali alespoň extensions pro každou entitu.</p><p></p><p>Lazy loading:</p><p></p><p>Podle mě přináší víc problémů než užitku. Ve svých aplikací ho explicitně vypínám a tam kde potřebuji donačíst externí data používám Include. Má to tu výhodu, že mě aplikace sama upozorní (null reference) když zapomenu donačíst potřebná data. Nevýhoda je, že člověk často skončí s velkým includem a tam je ten výkon pak taky omezený. Při zapnutém Lazy loadingu jsme často museli hledat profilerem a koukat, kde nám unikají dotazy (SELECT N+1)</p><p></p><p>Jinak za mě dva větší problémy:</p><p></p><p>Značná paměťová náročnost. Zvlášť když aplikace běží na sdíleném webhostingu a má omezené množství paměti RAM k dispozici.</p><p></p><p>Podpora hierarchické struktury. Složitější stromové struktury je potřeba řešit buď přes čisté SQL (Sql CTE) nebo se smířit s tím, že se načtou všechny záznamy a traverzování se provádí až v paměti.</p>', CAST(N'2016-05-05T14:34:49.537' AS DateTime), N'213.220.233.209')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (24, 8457, N'<p>Assembly ze streamu lze načíst nějak takhle:</p><p></p><pre class=""brush: csharp"">&#10;//IAssemblyLoadContextAccessor loaderAccessor&#10;//Stream assemblyData &#10;&#10;var loader = loaderAccessor.GetLoadContext(typeof(MyClass).GetTypeInfo().Assembly);&#10;var assembly = loader.LoadStream(assemblyData, null);&#10;</pre><p></p><p></p><p>kde loaderAccessor typu Microsoft.Framework.Runtime.IAssemblyLoadContextAccessor si je potřeba nechat resolvnout z DI kontejneru a assemblyData je stream assembly. MyClass je nějaká třída v našem projektu (určuje kontext pro načtení dalších assembly).</p>', CAST(N'2015-05-20T19:07:35.327' AS DateTime), N'89.177.81.102')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (25, 8496, N'<p>Bezvadný článek, jdu si ho vytisknout do tramvaje.</p><p>K záporům EF bych ještě přidala, že neumí pracovat s fulltextovým indexem &#8230;</p>', CAST(N'2016-05-05T15:58:53.677' AS DateTime), N'147.33.190.25')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (26, 8496, N'<p>Mám velmi podobné zkušenosti s podobným heavy frameworkem - NHibernate s tím, že jsem po po této zkušenosti velmi dogmatický a těžké frameworky mi do projektů nesmí.</p><p></p><p>Použili jsme NHibernate v rámci docela velkého projektu kde se množství dat v tabulkách pohybuje v řádech desítek až stovek miliónů a časem se nám to velmi vymstilo. Bylo to způsobeno jak počáteční nezkušeností a nadšením jak to &quot;celé funguje skvěle samo&quot;. Nadšení nás ale posléze přešlo v okamžiku kdy se začaly vynořovat výkonnostní a jiné problémy.</p><p></p><p>Vůbec teze skrýt před programátorem DB a dělat, že tam vlastně vůbec není považuji za jeden z největších omylů těchto FW. Ano funguje to na jednoduché aplikace s pár tabulkami a se stovkami řádků v nich. Tam je to opravdu fuk. Ale upřímně k čemu je programátor, který pořádně ani neumí napsat SQL dotaz aby nenačetl půlku DB :-) Z toho pak vycházejí další problémy, které zmiňujete. Neoptimalizované a špatně čitelné dotazy, které to generuje. Právě NHibernate na to byl expert, naprostá nečitelná hrůza v jednom řádku, něco v tom najít a zjistit proč je ten dotaz tak pomalý bylo peklo. A to co generoval Entity Framework v prvních verzích, to bylo opravdu něco.</p><p></p><p>Pokud programátora donutíte se nad tím dotazem zamyslet, než ho tam namastí, jak by mohl ho napsat aby dotaz použil existující indexy, jestli opravdu potřebuje všechny sloupce atd, je si myslím velice užitečné. Neříkám, že každý programátor má být zároveň DB specialistou, ale základní znalosti by mít měl. A právě skrytí DB těžkým frameworkem toto hatí.</p><p></p><p>Poslední rána je příchod nového člena do týmu, který nezná záludnosti použitého FW a díky špatnému mapingu, kaskády a lazy loadingu zjistíte, že máte v paměti půlku DB :-)</p><p></p><p>Z toho plyne moje poučení které s oblibou říkám: Tězký ORM FW je velmi jednoduché špatně použít.</p><p></p><p>Po několika pokusech a zkoumání jsme zůstali u FW BLToolkit. Má plně implementovanou podporu linqu včetně zajímavých extension, nehraje si na blackbox a DB neskrývá. Žádné sledování změn, žádný lazy loading, žádné kaskádové update a delete. Je to sice trochu pracnější na vývoj, ale dotazy to generuje hezké a čitelné a většinou je není ani potřeba přepisovat do čistého SQL. A v konečném důsledku čas ušetříte.</p><p>Od té doby na nás náš DB specialista neřve co to tam pouštíme za strašné hrůzy :-)</p>', CAST(N'2016-05-06T13:22:42.790' AS DateTime), N'86.49.185.50')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (27, 8496, N'<p>Mám s EF podobné zkušenosti, jako Vy. Chtěl bych se tedy zeptat, co používáte místo něho?</p><p>Na standardním ADO.NET mi vadí nutnost vše ručně načítat do objektů a také nutnost ruční implementace stránkování či projekce. </p><p></p><p>Máte tipy na nějakou alternative/nadstavbu ADO.NET která by tyto procesy automatizovala, nebo dokonce generovala dotazy pomocí LINQ?</p>', CAST(N'2016-05-07T18:52:52.090' AS DateTime), N'90.181.177.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (28, 8499, N'<p>Zdravim,</p><p>mozno by bolo fajn napisat aj navod ku Seleniovym testom pre ASP.NET MVC a ASP.NET Core.</p><p>Dost by som ich ocenil.</p>', CAST(N'2016-10-13T17:39:58.240' AS DateTime), N'78.98.56.224')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (29, 8511, N'<p>Lze nějakým způsobem přes C# Script API načíst do skompilovaného programu soubor se scriptem a volat metody v tomto scriptu? Jak lze načíst proměnné ze scriptu?</p><p>A naopak, lze z těchto metod v scriptu přistupovat k proměnným ze skompilovaného programu?</p>', CAST(N'2017-03-18T22:55:22.190' AS DateTime), N'95.140.250.45')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (30, 8520, N'<p>Jen škoda, že je licence jen na rok. Jiná volba není. Každý rok dávat necelých 180 USD (s Bootstrapem) no nevím...</p>', CAST(N'2017-02-28T13:26:06.750' AS DateTime), N'193.111.133.187')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (31, 8520, N'<p>U Boostrap for DotVVM je licence trvalá, během prvního roku máte updaty v ceně, ale knihovna samotná funguje i po vypršení té roční lhůty.</p><p></p><p>U doplňku do Visual Studia platí, že základní verze je zdarma. Pokročilé funkce jsou placené a tam je to opravdu jen na rok. Nicméně ta lepší IntelliSense ušetří mnoho dost času, takže v konečném důsledku se to vyplatí. </p><p></p><p>Pokud nám pomůžete (např. nějaký pull request, zajímavé nápady, pomoc s dokumentací) nebo pracujete na nějakém projektu pro školství či charitu, kontaktujte nás a poskytneme Vám licenci zdarma. :-)</p>', CAST(N'2017-03-01T16:00:15.570' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (32, 8496, N'<p>Zdravím,</p><p></p><p>používáme toto <a href=""https://github.com/igor-tkachev/bltoolkit"" rel=""nofollow"">https://github.com/igor-tkachev/bltoolki...</a> a zkušenosti velmi dobré. Jediná nevýhoda je, že už je to trochu letité a komunita už vázne. Což nám moc nevadí, máme to již zažité a děláme si i vlastní úpravy a rozšíření.</p><p></p><p>Druhý FW od stejného autora ve kterém jak píše vylepšil návrh původního BLToolkitu, ale ještě jsem detailně nezkoušel, ale je to velice podobné. Vzhledem ke kvalitám prvního FW bych tomu docela věřil.</p><p><a href=""https://github.com/linq2db/linq2db"" rel=""nofollow"">https://github.com/linq2db/linq2db</a></p>', CAST(N'2016-05-08T21:29:41.297' AS DateTime), N'94.113.121.235')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (33, 8496, N'<p>No, je vidět, že není framework jako framework :)</p><p></p><p>S tím, co je zde popsáno, lze jen plně souhlasit. Otázka zjednodušení vývoje bývá v konečném důsledku velmi zrádná a většinou vede k ještě větším problémům. Základní problém vidím právě v tom, že nějakým způsobem zjednodušit a zautomatizovat vývoj aplikace (programování) na pouhou &quot;klikačku&quot; je utopie. Bez znalostí jde vývoj většinou do kytek. </p><p></p><p>Právě začátek vývoje bývá problematický a hektický. Máme zadání, současně běží analýza, lidi nemají ujasněnu celkovou problematiku a koncepci, tak posedávají a čekají na základní vstupy pro svou práci. Čili řada programátorů čeká na vstupní podklady (tabulky, dotazy, koncepci tříd a pod.). Do toho je nervózní šéf, protože práce neodsýpá. Týden je v tahu a on nevidí žádný funkční formulář napojený na data. Použití částí kódu z předchozí aplikace je nemožné, protože se to vyvíjelo v poklusu, jsou tam chyby a je to nedomyšlené. Nemá cenu to znovu použít, vyvineme si to znovu (už asi po sedmé).</p><p></p><p>Čili všemi těmito problémy jsme si prošli také. Pak jsme začali koketovat s frameworkem Digital Data Cube. Na základě předchozích zkušeností samozřejmě s nedůvěrou. Tu jsme překonali a tak přes rok jej používáme. Je to jako s jinými nástroji. Než je pochopíte, vezme to nějaký čas. Taky u prvního projektu chybí řada věcí, které nejsou naprogramovány. Překonali jsme to a výsledek je vidět. Výše popsaných problémů jsme se zcela zbavili. Jakmile jsme napsali základní věci pod vedením lektora, práce odsýpají. A lidi to baví. Necítí se zbyteční a nevyžití. </p><p></p><p>Framework nám poskytnul věci, které bychom nemohli ani zanalyzovat, natož naprogramovat a implementovat do projektů v rámci jiných frameworků. Takže po této stránce jsme spokojeni. Výsledky jsou hmatatelné. Zrychlil se vývoj a dramaticky se snížila chybovost, což vždy tak zrovna jednoduché nebylo. Čili, v konečném výsledku, si nemůžeme stěžovat. Naopak jsme rádi, že jsme se rozhodli pro změnu. </p><p></p><p>Faktem ale je, že i v tomto případě se nejedná o pouhou &quot;klikačku&quot;. Zkrátka bez znalostí a zkušeností se neobejdete. I když je zautomatizována řada věcí, analýza je analýza a návrh databáze bude vždy alfou a omegou vývoje. Alespoň u databázových aplikací.</p>', CAST(N'2016-11-08T18:34:13.137' AS DateTime), N'85.207.4.133')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (34, 8496, N'<p>Fulltextové rozšíření - pravda. Také velký nedostatek.</p>', CAST(N'2016-05-05T16:45:51.283' AS DateTime), N'213.175.38.130')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (35, 8457, N'<p>Na to už jsem taky přišel, potíž je v tom, že v Roslynu plánují zrušit jednu funkci, kterou používám a která umožňuje vytáhnout metadata z assembly, takže to, na co potřebuju načíst assembly ze streamu, stejně budu dělat jinak - pátrám dál, co s tím.</p>', CAST(N'2015-05-27T19:54:23.680' AS DateTime), N'89.177.184.40')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (36, 8496, N'<p>Myslím, že v těch bodech se shodneme, v zásadě jsem i to samé psal v článku, jen jinými slovy. </p><p></p><p>S pamětí může být problém, pokud načteme mnoho záznamů najednou a nevypneme u nich proxy třídy / sledování. Jinak jsou to jen hloupé objekty a paměť neplýtváme.</p><p></p><p>K té hierarchii je to možná dobře. Databáze má ukládat &quot;jednoduchá data&quot; a nějaké rozhraní nad tím si zařídím v business vrstvě.</p>', CAST(N'2016-05-05T14:46:48.337' AS DateTime), N'213.175.38.130')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (37, 8491, N'<p>Máš pravdu, chybička se vloudila. :)</p>', CAST(N'2016-06-15T16:10:30.207' AS DateTime), N'185.91.165.23')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (38, 8457, N'<p>Aha, tak v CoreCLR je, ale v CoreFX ne. Zajímavé.</p>', CAST(N'2015-05-18T16:03:50.197' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (39, 8451, N'<p>My používáme TeamCity, takže s tím osobní zkušenost nemám. Zkuste ale <a href=""http://stackoverflow.com/questions/24618182/i-want-canopy-web-testing-results-to-show-in-vs-2013-test-explorer-and-im-so"" rel=""nofollow"">http://stackoverflow.com/questions/24618...</a>, vypadá to nadějně.</p>', CAST(N'2015-09-06T17:50:21.067' AS DateTime), N'77.240.182.82')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (40, 8451, N'<p>To nedává moc smysl, radši si to rovnou postav nad Seleniem. Tady je hlavní výhoda ta čitelnost F#, ten test ti pak napíše i projekťák. Trochu to připomíná akceptační testy ve SpecFlow. Testy jako takové jsou úplně v pohodě, spíš to potřebuje dopsat ještě větší sadu assertů a helprů pro práci s HTML a CSS. I těžší testy které něco odesílají, vyplňují a podobně jsou pěkně čitelné. A ještě by se hodilo porovnávání obrazu, které by mi dokázalo říct jestli screen odpovídá nějakému před definovanému.</p>', CAST(N'2015-04-30T18:22:26.900' AS DateTime), N'37.48.42.142')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (41, 8443, N'<p>Správně, přesně tak.</p>', CAST(N'2015-03-24T17:01:06.130' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (42, 8443, N'<p>Správně, přesně tak.</p>', CAST(N'2015-03-24T17:01:13.197' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (43, 7435, N'<p>jasně, to je dobrá připomínka, doplníme :-)</p>', CAST(N'2015-01-15T12:57:14.810' AS DateTime), N'89.177.158.177')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (44, 7435, N'<p>MonoGame je tak nějak v prostřed spíš blíž tomu nízkoúrovňovýmu, je to stejné jak XNA vše si musíš zařídit sám ale právě díky XNA je na webu spousta dobrých článků a dodatečných knihoven.</p><p>Grafiku můžeš importovat v podobě obrázků a modelů (fungují formáty .x a .fbx), ale všeobecně platí že pokaď chceš aby hra nějak vypadala, tak grafika určitě potřebuješ (já teda určitě :-D)</p>', CAST(N'2015-01-15T13:02:57.417' AS DateTime), N'89.177.158.177')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (45, 6415, N'<p>Nepřesnosti v názvech jsem opravil. Co se týče zbytku, o každém z odstavců článku se dá napsat podrobný článek, ale to by přesahovalo úroveň tohoto seriálu. Úmyslně jsem se to snažil zjednodušit tak, aby programátor, který začíná nebo přechází na C#, dostal výčet vlastností jazyka, které bude potřebovat k práci s LINQem a ne ho zavalit informacemi, které nutně nepotřebuje.</p>', CAST(N'2014-09-23T08:00:43.307' AS DateTime), N'83.240.30.77')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (46, 6395, N'<p>Nevím v čem vás PostSharp omezuje. </p><p></p><p>PostSharp používá pro připojení aspektů MSIL Injection, takže je na jazyku nezávislý. Budu to rozebírat v příštím článku. </p><p></p><p>Pokud metoda, v níž dojde k výjimce, výjimku rovnou odchytí, pak ji PostSharp nezaznamená, nemá jak protože PostSharp neupravuje kód metody, on ho pouze obalí dalším kódem, jak bylo znázorněno v prvním článku. V případě že chcete zapisovat do logu i zachycené výjimky nic vám nebrání zapsat si ji do logu přímo v kódu. PostSharp nepokryje stoprocentně všechny případy, ale ušetří vám velké množství opakujícího se kódu.</p>', CAST(N'2014-07-23T14:20:15.907' AS DateTime), N'81.161.64.26')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (47, 6393, N'<p>Takže autory .NET frameworku a LINQ byste také radši k psaní kódu nepouštěl? Protože na funkcích velmi podobných té, které kritizujete, je postaven celý LINQ, podívejte se na signaturu funkce GroupBy nebo Join. </p><p></p><p>A to přesunutí kódu, o kterém mluvíte, není z jednoho místa A na jedno místo B, ale z tisíce míst A na jedno místo B - aspekty elegantně řeší exception handling, logování a mnoho dalších věcí, takže místo toho, abyste měl v aplikaci tisíc try-catch bloků, máte prostě jeden atribut, případně ty aspekty můžete aplikovat podle nějakých konvencí a obejdete se bez atributu úplně.</p>', CAST(N'2014-07-09T21:11:33.103' AS DateTime), N'88.208.76.111')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (48, 6393, N'<p>Věru mě nezajímá, jak je Framework napsaný uvnitř, ani sám žádný nepíšu. V obyčejných aplikacích by se takový humus těžko někde našel.</p><p></p><p>Co se týče logování. Čím bude lepší toto než způsob, jaký používám já: Všude kde je potřeba něco zaznamenat, zavolám potřebnou metodu ze singleton třídy a je hotovo. V případě nutnosti změny logovacího mechanizmu provedu změnu na jediném místě.</p><p></p><p>Co se týče řešení vyjímek. Mějme dejme tomu dvacet, třicet míst, na kterém je potřeba řešit vyjímky trochu jiným způsobem, nelze tedy použít něco univerzálního. Jak by tohle pomohlo, aniž by bylo nutné srát se s implementací nějaké (jistě jako obvykle zbytečně komplikované) infrastruktury a tím ztrácet čas?</p>', CAST(N'2014-07-09T23:07:19.880' AS DateTime), N'77.48.126.106')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (49, 8443, N'<p>Osobně tedy větu &quot;Nesmíte změnit to, jak vypadají vlastnosti Domain, Port a Name - musí i nadále volat tu funkci GetValue bez parametru.&quot; chápu tak, že funkce by neměla mít parametr vůbec, tedy ani nepovinný...</p>', CAST(N'2015-03-24T17:07:17.280' AS DateTime), N'194.108.204.148')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (50, 8451, N'<p>No mě to v tom F# právě nepřijde čitelné ani trochu, je to spíš zmatené. Ale to je asi o zvyku.</p>', CAST(N'2015-04-30T18:33:59.630' AS DateTime), N'193.86.188.36')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (51, 8457, N'<p>Co jsem to pochopil, tak CoreFX mscorlib vůbec neobsahuje. To je věc CoreCLR. A netuším proč se to nechce zkompilovat při použití nějaké té metody, když CoreCLR i normální velký CLR je podporuje. Nemohlo bz to být něco s WinRT? Protože všechny hacky jak to obejít co jsem našel řešily tyhle metro aplikace.</p>', CAST(N'2015-05-19T08:01:54.917' AS DateTime), N'217.30.72.69')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (52, 8520, N'<p>A co třeba pro další používání po tom roce upgrade programátor/rok cca 30 USD? To si myslím, že by nebyla špatná varianta. Například pro mě by to byla motivace se DotVVM naučit a používat :-)</p>', CAST(N'2017-03-13T14:23:49.957' AS DateTime), N'193.111.133.187')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (53, 8451, N'<p>Kdyby, všechny testy vypadali jako tento:</p><p></p><pre class=""brush: csharp"">&#10;&quot;search Oprava laveček v nádražní budově Kr.Pole&quot; &amp;&amp;&amp; fun _ -&gt;&#10;    url &quot;http://www.lepsimisto.cz/&quot;&#10;    &quot;#searching&quot; &lt;&lt; &quot;Oprava laveček v nádražní budově Kr.Pole&quot;&#10;    press enter&#10;&#10;    takeScreenshot &quot;search&quot;&#10;&#10;    click &quot;section.search-results article.bottom div.content div.item h3 a&quot;&#10;    on &quot;http://www.lepsimisto.cz/tip/oprava-lavecek-v-nadrazni-budove-krpole&quot;&#10;&#10;&#10;</pre><p></p><p>Tak by to byla paráda, zkusím pár assertu dopsat a poslat jim pull request</p>', CAST(N'2015-04-30T19:35:50.527' AS DateTime), N'78.128.215.4')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (54, 6393, N'<p>Jak je Linq napsaný uvnitř, vás nemusí zajímat pouze do té doby, než ho chcete více využívat a nebo napsat nějaké jeho rozšiřující metody. Navíc ta metoda v ukázce vůbec není podstatná.</p><p></p><p>Když chcete do logu zaznamenat výjimku tak ji musíte nejdřív odchytit, zapsat do logu a poté, pokud ji neumíte zpracovat, poslat dál a hlavně musíte pamatovat na to že se má v daném místě logovat. To je hodně kódu navíc a musíte spoléhat na to že máte v logu opravdu vše, rači tam toho mám víc a vyfiltruju si to, než zjišťovat co tam není.</p><p></p><p>Infrastruktura je zbytečná leda na malých projektech. Na velkém projektu se vám určitě vrátí, pokud máte dostatečně inteligentní vývojáře, pokud architekturu a použité technologie tým nechápe a použije je špatně, pak je to opravdu ztrácení času.</p>', CAST(N'2014-07-12T21:22:03.590' AS DateTime), N'194.228.32.104')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (55, 6393, N'<p>Takže pokud vám v LINQ nějaká metoda chybí, tak si ji prostě nenapíšete a budete čekat do další verze, jestli se MS nesmiluje a neudělá vám ji tam? Zřejmě píšete velmi jednoduché aplikace a ještě k tomu sám. Logovat pomocí singleton objektu je taky vhodné pouze v malých projektech, může to dělat problémy jak při testování (na což máte asi taky malé projekty) anebo ve chvíli, kdy potřebujete část věcí logovat na jedno místo, část věcí na jiné atd., například kvůli tomu, že ne každý člověk v týmu má přístup na produkční server a nechcete mu ukazovat úplně všechny logy, ale třeba jen ty, které neobsahují žádná citlivá data apod., takových nebo podobných situací jsem zažil desítky.</p><p></p><p>Ano, pokud při každé výjimce potřebujete udělat něco jiného, pak se aspekty použít nedají. Akorát já jsem tak v 90% projektů potkal spíš opačnou situaci - výjimka může nastat na desítkách nebo stovkách míst a potřebuji se zachovat stejně - zalogovat, co se stalo, pak uživateli zobrazit okénko, že nastala nějaká chyba, a pokud jsem uvnitř nějaké transakce, tak udělat rollback. </p><p></p><p>A pozor na to, i z malých projektů bez infrastruktury se po pár letech někdy stávají velké projekty, kde ta infrastruktura bude citelně chybět. A přesvědčit zákazníka, že teď už by to vážně chtělo přepsat, není vždy jednoduché.</p>', CAST(N'2014-07-13T14:01:46.027' AS DateTime), N'88.208.76.111')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (56, 6393, N'<p>- LINQ je sám o sobě tak mocný, že jsem nikdy neměl potřebu ho rozšiřovat.</p><p></p><p>- Pokud potřebuju logovat na více různých míst, stačí mi přidat potřebného TraceListenera do konfiguračního souboru.</p><p></p><p>- Zřejmě píšete velmi jednoduché věci, pokud se všude chováte k vyjímkám stejně. To já ji někde potřebuju zaznamenat, někde zahodit, někde zobrazit zprávu uživateli, jinde ne.</p><p></p><p>- Ještě nikdy jsem nepsal věc, která by později nešla lehce rozšířit a to bez jakékoliv infrastruktury nutné pro podobná zvěrstva.</p><p></p><p>- Testy nepíšu, jsem odpůrce unit testů. Uživatelské rozhraní (část, která potřebuje testovat nejvíce) se testuje extrémě špatně a u aplikací, jaké píšu já, by bylo napsání efektivního testu aplikační logiky mnohem složitější, než napsání samotné aplikace. A testování je o podvržení nějaké funkční části testovacím zmetkem a už to mi připadá k smíchu, protože se to nikdy ani nebude blížit skutečnému provozu.</p>', CAST(N'2014-07-19T12:49:17.887' AS DateTime), N'77.48.126.106')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (57, 8496, N'<p>Aplikace Vás &quot;upozorní&quot; až za běhu s tím, že spadne a to ještě jen tehdy, když zapomenete načíst obyčejnou property. Pokud někde zapomenete načíst kolekci, tak nic nespadne, ale v aplikaci začnou zdánlivě záhadně mizet a objevovat se data.</p><p></p><p>Já nechávám lazy loading implicitně zapnutý a Includy většinou přidávám až nakonec, v rámci optimalizace výkonu. Výhodou tohoto přístupu je, že aplikace nikdy nespadne kvůli tomu, že jsem někde zapomněl Include.</p>', CAST(N'2016-05-05T18:32:00.767' AS DateTime), N'212.111.29.98')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (58, 6393, N'<p>souhlasím s Vámi.</p><p>Zmiňovanému kódu nepomůže AOP, ale komentář.</p><p></p><p>Log co mi zaloguje vyjimku je mi na prd, když k tomu nepřibalí věci, kvůli kterým vyjimka vznikla. Předpokladem pro toto tvrzení je že AOP nemá věšteckou kouli.</p>', CAST(N'2014-07-14T12:02:48.843' AS DateTime), N'213.216.60.190')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (59, 6393, N'<p>Nevím jak by komentář pomohl, ta metoda není složitá, jen dělá víc věcí najednou.</p><p></p><p>AOP opravdu nemá věšteckou kouli, ale má IL kód a z toho si zjistí vše co potřebuje. Jak logovat pomocí Log4Net pomocí PostSharpu se můžete podívat v druhém článku. Myslím si že takovéhle logování je dostatečné <a href=""http://www.dotnetportal.cz/blogy/16/Martin-Dybal/6395/Aspektove-orientovane-programovani-Logovani"" rel=""nofollow"">http://www.dotnetportal.cz/blogy/16/Mart...</a></p>', CAST(N'2014-07-16T11:26:20.167' AS DateTime), N'81.161.64.26')
INSERT [dbo].[Comments] ([Id], [ArticleId], [Html], [CreatedDate], [IpAddress]) VALUES (60, 6393, N'<p>Nikdy jsem neslyšel horší názor a postoj než ten, že komentář prozradí v rámci troubleshootingu více jak samotný log aplikace. Buďto jste nikdy nedělal troubleshooting nebo neprogramujete pro firemní oblasti, ale pro vlastní zájmy.</p>', CAST(N'2014-07-16T14:44:42.210' AS DateTime), N'88.208.88.196')
SET IDENTITY_INSERT [dbo].[Comments] OFF
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'admin', N'admin@test.com', 1, NULL, N'9644A231-0961-4CC2-9074-0E6D4BA2BC21', N'', 0, 0, NULL, 0, 0, N'admin')
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'tomas', N'tomas@test.com', 1, NULL, N'FDCC36F1-C661-487D-B4F0-B2E91C59076A', N'', 0, 0, NULL, 0, 0, N'tomas')
SET IDENTITY_INSERT [dbo].[BlogAuthors] ON 

INSERT [dbo].[BlogAuthors] ([Id], [BlogId], [IdentityUserId]) VALUES (2, 3, N'tomas')
SET IDENTITY_INSERT [dbo].[BlogAuthors] OFF
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'administrator', N'administrator')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'admin', N'administrator')
");
        }
        
        public override void Down()
        {
        }
    }
}
