# 🥐 BakeryApp — Система учёта пекарни

Пошаговое руководство по установке и запуску.

---

## 📋 Что требуется

| Компонент | Версия | Где взять |
|-----------|--------|-----------|
| Visual Studio | 2019 или 2022 (Community бесплатна) | visualstudio.microsoft.com |
| .NET Framework | 4.8 (скорее всего уже установлен) | Входит в Windows 10/11 |
| SQL Server | любая версия (Express бесплатен) | microsoft.com/sql-server |
| SQL Server Management Studio | любая | aka.ms/ssms |

---

## ШАГ 1 — Создать базу данных

1. Откройте **SQL Server Management Studio (SSMS)**
2. Подключитесь к вашему серверу
3. В меню: **File → Open → File...** — выберите файл `BakeryApp\SQL\CreateDatabase.sql`
4. Нажмите **F5** (или кнопку Execute / Выполнить)
5. Убедитесь, что в Messages написано:
   ```
   База данных BakeryDB создана.
   Таблица Ingredients создана.
   Таблица Products создана.
   Таблица ProductionRecords создана.
   Таблица SaleRecords создана.
   === BakeryDB успешно инициализирована ===
   ```

> Если скрипт уже запускался раньше — он безопасно пропустит создание существующих объектов.

---

## ШАГ 2 — Настроить строку подключения

Откройте файл `BakeryApp\App.config` в любом текстовом редакторе.

Найдите строку:
```xml
<add name="BakeryDB"
     connectionString="Server=localhost\SQLEXPRESS;Database=BakeryDB;Integrated Security=True;"
     providerName="System.Data.SqlClient" />
```

Замените `Server=...` на адрес вашего SQL Server. Примеры:

| Сценарий | Строка |
|----------|--------|
| SQL Server Express (по умолчанию) | `Server=localhost\SQLEXPRESS;Database=BakeryDB;Integrated Security=True;` |
| SQL Server на стандартном порту | `Server=localhost;Database=BakeryDB;Integrated Security=True;` |
| Именованный экземпляр | `Server=МОЙ_ПК\SQLEXPRESS;Database=BakeryDB;Integrated Security=True;` |
| Логин + пароль (SQL Auth) | `Server=localhost;Database=BakeryDB;User Id=sa;Password=МойПароль;` |

> **Integrated Security=True** означает вход под вашей учёткой Windows — это удобно и безопасно.
> Имя своего экземпляра SQL Server можно посмотреть в SSMS в поле "Server name" при подключении.

---

## ШАГ 3 — Открыть проект в Visual Studio

1. Запустите **Visual Studio**
2. **File → Open → Project/Solution...**
3. Найдите и выберите файл **`BakeryApp.sln`** (он лежит рядом с папкой BakeryApp)
4. Visual Studio откроет проект

---

## ШАГ 4 — Восстановить NuGet-пакет

Проект использует один NuGet-пакет: **System.Data.SqlClient** (современный SQL-клиент).

**Автоматически:**
- В Visual Studio: **Tools → NuGet Package Manager → Manage NuGet Packages for Solution**
- Нажмите **Restore** (или он сам предложит восстановить при открытии)

**Вручную (если не работает автовосстановление):**
1. **Tools → NuGet Package Manager → Package Manager Console**
2. Выполните команду:
   ```
   Install-Package System.Data.SqlClient -Version 4.8.3
   ```

> ⚠️ Без этого пакета код не скомпилируется — он нужен для работы с SQL Server.

---

## ШАГ 5 — Собрать и запустить

1. В Visual Studio нажмите **F6** (Build Solution) — убедитесь, что нет ошибок
2. Нажмите **F5** (Start Debugging) — приложение запустится

При первом запуске приложение само проверяет подключение и создаёт таблицы (если они ещё не созданы через SSMS).

Если подключение не работает — появится сообщение об ошибке с подсказкой.

---

## Структура проекта

```
BakeryApp.sln                    ← Файл решения (открывать этот)
│
└── BakeryApp\
    ├── App.xaml / App.xaml.cs   ← Точка входа, глобальные стили
    ├── App.config               ← Строка подключения к БД (МЕНЯТЬ ЗДЕСЬ)
    ├── BakeryApp.csproj         ← Файл проекта
    │
    ├── SQL\
    │   └── CreateDatabase.sql   ← Скрипт создания БД (запустить в SSMS)
    │
    ├── Models\                  ← Классы данных (POCO)
    │   ├── Ingredient.cs
    │   ├── Product.cs
    │   ├── ProductionRecord.cs
    │   └── SaleRecord.cs
    │
    ├── Data\                    ← Работа с базой данных (SQL-запросы)
    │   ├── DatabaseHelper.cs    ← Подключение, создание таблиц
    │   ├── IngredientRepository.cs
    │   ├── ProductRepository.cs
    │   ├── ProductionRepository.cs
    │   └── SaleRepository.cs
    │
    └── Views\
        ├── MainWindow.xaml/.cs  ← Главное окно с 4 вкладками
        │
        ├── Tabs\                ← Вкладки (UserControl)
        │   ├── IngredientsTab.xaml/.cs   ← Учёт ингредиентов
        │   ├── ProductsTab.xaml/.cs      ← Справочник продукции
        │   ├── ProductionTab.xaml/.cs    ← Журнал производства
        │   └── SalesTab.xaml/.cs        ← Журнал продаж
        │
        └── Dialogs\             ← Диалоговые окна редактирования
            ├── QuickQuantityDialog.xaml/.cs  ← Приход/Расход ингредиента
            ├── IngredientDialog.xaml/.cs
            ├── ProductDialog.xaml/.cs
            ├── ProductionDialog.xaml/.cs
            └── SaleDialog.xaml/.cs
```

---

## Что умеет приложение

### 📦 Ингредиенты
- Список ингредиентов: название, единица измерения, количество, закупочная цена
- Кнопки **«Приход»** и **«Расход»** — открывают маленькое окошко, куда вводите число, оно прибавляется/вычитается из остатка
- Поиск по названию, редактирование и удаление записей

### 🍞 Продукция
- Справочник того, что пекарня выпускает: название, тип (Хлеб/Булка/Пирожное/...), цена продажи, заметки
- Поиск по названию, типу и заметкам

### 🏭 Производство
- Журнал выпуска: дата, продукция, количество, комментарий
- Фильтр по диапазону дат (два календарика «С» и «По»)
- Поиск по названию продукции и комментарию

### 💰 Продажи
- Журнал продаж: дата, продукция, количество, цена за единицу, итого, покупатель
- При выборе продукции из справочника подставляется её цена (можно изменить)
- Поле «Итого» рассчитывается автоматически из количества × цену
- Фильтр по датам и поиск
- Внизу показывается **сумма продаж по текущему фильтру**

---

## Частые проблемы

**«Не удалось подключиться к базе данных»**
→ Проверьте строку подключения в App.config. Запустите SSMS и убедитесь, что можете подключиться к тому же серверу.

**«System.Data.SqlClient не найден»**
→ Восстановите NuGet-пакеты (ШАГ 4).

**Приложение собирается, но вылетает при запуске**
→ Убедитесь, что SQL Server запущен (Services → SQL Server (SQLEXPRESS) → Running).

**«Ошибка входа для пользователя»**
→ Либо используйте `Integrated Security=True`, либо проверьте логин/пароль в строке подключения.
