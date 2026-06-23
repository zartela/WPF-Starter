using CSVImportExportApp.Data;
using CSVImportExportApp.Models;
using CSVImportExportApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CSVImportExportApp;

public partial class MainForm : Form
{
    private readonly CsvImportService _importService;
    private readonly DataQueryService _queryService;
    private readonly ExcelExportService _excelExport;
    private readonly XmlExportService _xmlExport;

    public MainForm()
    {
        InitializeComponent();
        var context = new AppDbContext();
        _importService = new CsvImportService(context);
        _queryService = new DataQueryService(context);
        _excelExport = new ExcelExportService();
        _xmlExport = new XmlExportService();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        dateTimePickerFrom.Value = DateTime.Now;
        dateTimePickerTo.Value = DateTime.Now;
        LoadData();
    }

    private async void btnLoadCsv_Click(object sender, EventArgs e)
    {
        using OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "CSV files (*.csv)|*.csv";
        openFileDialog.Title = "Выберите CSV файл";

        if (openFileDialog.ShowDialog() != DialogResult.OK) return;

        try
        {
            await _importService.ClearTableAsync();
            var total = await _importService.ImportAsync(openFileDialog.FileName);

            MessageBox.Show($"Загрузка завершена!\nИмпортировано: {total} записей",
                "Импорт CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке CSV: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnApplyFilter_Click(object sender, EventArgs e)
    {
        LoadData();
    }

    private void btnClearFilter_Click(object sender, EventArgs e)
    {
        cbFromDate.Checked = true;
        cbToDate.Checked = true;
        dateTimePickerFrom.Value = DateTime.Now;
        dateTimePickerTo.Value = DateTime.Now;
        txtCity.Clear();
        txtLastName.Clear();
        txtFirstName.Clear();
        txtSurName.Clear();
        txtCountry.Clear();
    }

    private async void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            var filters = GetFilters();
            var total = await _queryService.GetTotalCountAsync(filters);

            if (total == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            await _excelExport.ExportAsync(_queryService.GetQuery(filters), saveFileDialog.FileName);

            MessageBox.Show($"Экспорт завершён!\nСохранено {total} записей",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnExportXml_Click(object sender, EventArgs e)
    {
        try
        {
            var filters = GetFilters();
            var total = await _queryService.GetTotalCountAsync(filters);

            if (total == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            await _xmlExport.ExportAsync(_queryService.GetQuery(filters), saveFileDialog.FileName);
            
            MessageBox.Show($"Экспорт завершён!\nСохранено {total} записей",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка экспорта в XML: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void LoadData()
    {
        try
        {
            var filters = GetFilters();
            var items = await _queryService.GetQuery(filters)
                .OrderByDescending(p => p.Id)
                .Take(100)
                .ToListAsync();

            dataGridView1.DataSource = items;

            if (dataGridView1.Columns["Id"] != null)
                dataGridView1.Columns["Id"].Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private QueryFilters GetFilters()
    {
        return new QueryFilters
        {
            FromDate = cbFromDate.Checked ? dateTimePickerFrom.Value.Date : null,
            ToDate = cbToDate.Checked ? dateTimePickerTo.Value.Date : null,
            City = string.IsNullOrWhiteSpace(txtCity.Text) ? null : txtCity.Text.Trim(),
            LastName = string.IsNullOrWhiteSpace(txtLastName.Text) ? null : txtLastName.Text.Trim(),
            FirstName = string.IsNullOrWhiteSpace(txtFirstName.Text) ? null : txtFirstName.Text.Trim(),
            SurName = string.IsNullOrWhiteSpace(txtSurName.Text) ? null : txtSurName.Text.Trim(),
            Country = string.IsNullOrWhiteSpace(txtCountry.Text) ? null : txtCountry.Text.Trim()
        };
    }
}
