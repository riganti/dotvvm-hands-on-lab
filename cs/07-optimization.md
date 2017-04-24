## 7 Jak to vlastně funguje

Nyní je na čase říct si, jak v DotVVM fungují commandy. 

Ve chvíli, kdy uživatel poprvé přijde na stránku, prohlížeč vystaví *HTTP GET* požadavek. DotVVM najde v routovací tabulce příslušnou stránku, vytvoří instanci viewmodelu, a následně na něm zavolá události `Init`, `Load` a `PreRender`. Pak viewmodel vyserializuje do JSONu, vygeneruje HTML kód stránky, a pošle jej jako odpověď. 

Součástí HTML kódu stránky je jednak JSON reprezentace viewmodelu a dále odkaz na JavaScriptové knihovny `dotvvm.js` a `knockout.js`. JavaScriptová část DotVVM viewmodel načte a předhodí jej knihovně Knockout, která stránku "oživí". DotVVM komponenty totiž překládají data-bindingy do podoby, které Knockout JS rozumí. Pro `<dot:TextBox Text="{value: Something}" />` totiž vyrenderují `<input type="text" data-bind="value: Something" />`. 

Knockout JS zároveň sleduje, jestli se hodnoty ve viewmodelu nemění, a pokud ano, aktualizuje podle toho komponenty a elementy ve stránce. Pokud naopak uživatel něco napíše do textového pole, Knockout JS tuto změnu uloží do viewmodelu. Díky tomu viewmodel obsahuje vždy stav celé stránky - data-binding je obousměrný.

Ve chvíli, kdy uživatel klikne na tlačítko, které volá funkci `PostCommand` ve viewmodelu, `dotvvm.js` vezme viewmodel, převede jej na JSON a pošle jej na server jako *AJAXový HTTP POST požadavek*. Na serveru se opět dohledá v routovací tabulce správná stránka a viewmodel. 

DotVVM vytvoří novou instanci viewmodelu a zavolá událost `Init`. V této události ještě ve viewmodelu na serveru nejsou vidět změny, které v něm udělal klient. 

Po fázi `Init` DotVVM deserializuje viewmodel z JSONu a nastaví hodnoty vlastností do serverového viewmodelu. Poté zavolá funkci `Load`, kde jsou již změny od klienta vidět.

Po fází `Load` se ve stránce dohledá komponenta, která HTTP požadavek vyvolala (tzv. "postback") a zavolá se funkce `PostCommand`, na níž byla tato komponenta navázána.

Pak teprve přijde na řadu funkce `PreRender`, ve které načítáme data z databáze. V tuto chvíli již totiž komentář v databázi bude, a to, co z ní načteme, bude odpovídat aktuálnímu stavu. 

Nakonec DotVVM vyserializuje viewmodel zpět do JSONu a pošle jej jako odpověď na HTTP požadavek. HTML se v této fázi již negeneruje, výsledkem je pouze JSON. Klientská část DotVVM jej načte a upraví klientský viewmodel, načež Knockout JS zareaguje a adekvátně upraví elementy ve stránce.

### 7.1 Optimalizace přenášených dat

Abychom vždy nemuseli přenášet celý viewmodel, existují v DotVVM prostředky, kterými můžete řídit, která část se bude přenášet kterým směrem.

Text článku stačí z databáze načítat pouze při prvním načtení stránky. Mohli bychom jej navíc vyrenderovat přímo jako HTML, protože se nikdy nemění, a tím pádem bychom jej ani nemuseli dávat do viewmodelu. Díky tomu se nebude muset přenášet tam a zpět ve chvíli, kdy uživatel přidává komentář.

Ve viewmodelu můžeme u vlastností použít atribut `[Bind(Direction = ...)]`, čímž říkáme, kterým směrem se daná hodnota přenáší. Tímto atributem bychom mohli odekorovat vlastnost `Html` v objektu `ArticleDetailDTO` a nastavit, že se nebude přenášet na klienta ani zpět. 

Abychom to mohli udělat, musíme do business vrstvy přidat Nuget balíček `DotVVM.Core`, kde je atribut `Bind` definován. Tento balíček obsahuje jen několik atributů a rozhraní, která se mohou hodit v bussiness vrstvě. Zbytek frameworku je obsažen v balíčku `DotVVM`, ten ale do business vrstvy dávat nechceme.

> Otevřete soubor `DotvvmBlog\packages.config` a podívejte se, v jaké verzi je v projektu nainstalován balíček `DotVVM.Core`.

> V okně *Package Manager Console* vyberte projekt `DotvvmBlog.BL` a spusťte příkaz `Install-Package DotVVM.Core -version XXX`. Za `XXX` dosaďte stejnou verzi, kterou jste našli v `packages.config`.

> Nyní najděte třídu `ArticleDetailDTO` a vlastnost `Html` odekorujte následujícím atributem:

```
    [Bind(Direction.None)]
    public string Html { get; set; }
```

> Bude třeba přidat následující `using` klauzuli:

```
using DotVVM.Framework.ViewModel;
```

Nyní musíme zajistit, aby DotVVM komponenta `HtmlLiteral` nevyrenderovala Knockout JS data-binding, ale aby HTML vypsala přímo do stránky při jejím prvním načtení.

> Ve stránce `ArticleDetail.dothtml` přidejte komponentě `HtmlLiteral` vypisující `Html` článku vlastnost `RenderSettings.Mode`:

```
    <dot:HtmlLiteral Html="{value: Html}" RenderSettings.Mode="Server"
                        class="article-content" />
```

Tím jsme docílili snížení objemu přenášených dat při postbacku. Podobně bychom mohli upravit i ostatní vlastnosti z objektu `Article`, nicméně ty budou zabírat jen stovky bajtů, takže se to nijak zásadně nevyplatí.

V aktuální verzi DotVVM není možné nastavit `Direction.None` na celou vlastnost `Article` - DotVVM zatím neumí zpracovat vlastnost `DataContext` v případě, že hodnota ve viewmodelu není definována vůbec. Ve verzi 1.2 to již bude možné, díky čemuž by šlo zakázat přenos celému objektu `Article` a server rendering zapnout celému `div` elementu, který článek renderuje.

### 7.2 Načítání dat o článku

Ve viewmodelu je také jistý prostor pro optimalizaci. Objekt `Article` načítáme při každém požadavku, přestože to není potřeba. Hodnoty se uchovávají ve viewmodelu a přenášíme je tak jako tak, takže by článek stačilo načíst jen při prvotním GET požadavku.

> Upravte kód metody `PreRender` takto:

```
    public override Task PreRender()
    {
        var articleId = Convert.ToInt32(Context.Parameters["Id"]);

        var service = new ArticleDetailService();

        if (!Context.IsPostBack)
        {
            Article = service.GetArticle(articleId);
        }

        Comments = service.GetComments(articleId);

        return base.PreRender();
    }
```

Vlastnost `Context.IsPostBack` bude mít hodnotu `true` v případě, že se jedná o postback. Článek načítáme z databáze pouze v případě, že se o postback nejedná.
