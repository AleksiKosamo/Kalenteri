# Kalenteri Näyttötyö

## Toiminnan Selvitys ja Käyttötarkoitus

Kalenteri Näyttötyö on yksinkertainen Windows-sovellus, jonka avulla käyttäjä voi pitää muistiinpanoja eri päivämääristä. Sovellus tarjoaa käyttäjälle kalenterinäkymän, jossa hän voi klikata päivämääriä ja lisätä niihin muistiinpanojaan. Pääkäyttötarkoitus on helpottaa päivittäisten tehtävien ja tapahtumien hallintaa.

## Vuokaavio

Sovelluksen toiminnasta ei ole saatavilla vuokaaviota tällä hetkellä.

## Kuvakaappaukset

Sovelluksen käyttöliittymän kuvakaappauksia ei ole saatavilla tällä hetkellä.

## Pääkohdat Koodista

### Timer_Tick -metodi

```csharp
// Ajastimen tikin käsittelijä
private void Timer_Tick(object sender, EventArgs e)
{
    UpdateCurrentTime();
}
```

### ShowDaysBasedOnCurrentMonth -metodi

```csharp
// Näyttää päivät kuukauden mukaisesti
private void ShowDaysBasedOnCurrentMonth()
{
    int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

    int maxVisibleDay = Math.Min(daysInMonth, 31);

    // Piilottaa päivät, jotka eivät kuulu nykyiseen kuukauteen
    for (int i = 1; i <= 31; i++)
    {
        if (i <= maxVisibleDay)
        {
            Controls["day" + i].Visible = true;
        }
        else
        {
            Controls["day" + i].Visible = false;
        }
    }
}
```

## Jatkokehitysideat

- Mahdollisuus tallentaa muistiinpanot eri tiedostoihin eri käyttäjille.
- Lisää muotoiluvaihtoehtoja muistiinpanojen sisällölle, kuten lihavointi, kursivointi jne.
- Parempi virheenkäsittely ja lokitiedostot virhetilanteiden varalta.
- Synkronointimahdollisuus muiden laitteiden kanssa.

