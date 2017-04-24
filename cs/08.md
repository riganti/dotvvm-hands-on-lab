## 8 Validace

Jednu z věcí, kterou jsme ještě neřešili, je validace. Aktuálně totiž uživatel může přidat prázdný komentář.

DotVVM umí pro validaci využívat Data Annotations atributy z .NET Frameworku.

> Otevřete třídu `NewCommentDTO` a vlastnost `Text` odekorujte následujícím atributem:

```
    [Required]
    public string Text { get; set; }
```

Každý postback v DotVVM standardně validuje celý viewmodel a pokud některá vlastnost není validní, postback se neprovede.
Validaci je možné pro tlačítko vypnout pomocí vlastnosti `Validation.Enabled="false"`, případně můžete pomocí vlastnost `Validation.Target="{value: NewComment}"`
říci, že se bude validovat jen část viewmodelu. Musí se jednat o objekt, nelze target nastavit na primitivní datové typy jako `string` nebo `int`.

### 8.1 ValidationSummary

Aby uživatel viděl, proč se komentář nepřidal, musíme do stránky přidat komponenty, které validační chyby zobrazují.

Nejjednodušší je použít komponentu `ValidationSummary`. Ta zobrazí seznam všech validačních chyb ve svém `Validation.Target`. 

`ValidationSummary` zobrazuje validační chyby jen z targetu, ne z objektů, které jsou v něm obsažené. To se dá změnit vlastností `IncludeErrorsFromChildren`.

Před `div` obsahující tlačítko pro odeslání formuláře tedy přidejte následující blok:

```
    <div class="form-group validation">
        <dot:ValidationSummary Validation.Target="{value: _this}" />
    </div>
```

> Spusťte aplikaci a zkuste přidat prázdný komentář. Měla by se objevit chybová hláška.

Mimo to se v rohu obrazovky objeví následující hláška. Ta se objevuje pouze v _debug_ režimu, aby si vývojář uvědomil, že postback se neprovádí kvůli validaci. Pokud byste totiž do stránky žádnou validační komponentu nedali, tlačítko by nic nedělalo a nemuselo by být hned jasné, že je to kvůli validaci.

<img src="08-validation-debug.png" alt="Debug validační hláška" />

Pokud aplikaci zkompilujete v _release_ módu, hláška se neobjeví. Řídí se to nastavením `configuration.Debug` v souboru `Startup.cs`.

### 8.2 Validator

Občas můžeme chtít zobrazit chybovou hlášku hned vedle příslušného políčka. Můžeme použít komponentu `Validator`:

```
<dot:Validator Value="{value: Text}" class="validation" />
```

Chybová hláška se zobrazí v případě, že je vlastnost `Text` nevalidní. Komponenta `Validator` má ještě další nastavení, a nemusíte ji používat jako komponentu.

Pokud bychom chtěli například `div`u, uvnitř kterého je náš `TextBox`, nastavit nějakou CSS třídu v případě, že je `Text` nevalidní, udělali bychom to takto:

```
    <div class="form-group">
        <div Validator.Value="{value: Text}" Validator.InvalidCssClass="has-error">
            <dot:TextBox Text="{value: Text}" Type="MultiLine"
                            class="form-control" style="height: 140px" />
        </div>
    </div>
```

Vlastnost `Validator.Value` říká, která vlastnost se má kontrolovat, a `Validator.InvalidCssClass` říká CSS třídu, která se má přidat v případě, že je vlastnost nevalidní. V případě Bootstrapu používáme CSS třídu `has-error`, která pole obarví červeně.

Vlastnost `Validator.InvalidCssClass` nemusíte nastavovat u každého takového `div`u - stačí ji nastavit na libovolném nadřazeném elementu, klidně třeba na elementu `body` v master page.

> Upravte `div` s textovým polem pro zadání formuláře ve stránce `ArticleDetail.dothtml` takto:

```
    <div class="form-group">
        <div Validator.Value="{value: Text}">
            <dot:TextBox Text="{value: Text}" Type="MultiLine"
                            class="form-control" style="height: 140px" />
        </div>
    </div>
```

> V souboru `Site.dotmaster` nastavte elementu `body` vlastnost `Validator.InvalidCssClass`:

```
<body Validator.InvalidCssClass="has-error">
```

Nyní stačí kdekoliv v aplikaci libovolnému elementu nastavit `Validator.Value` a DotVVM na něj přidá nebo odebere CSS třídu `has-error`. 
