﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Blocknot
{
    public partial class Form1 : Form
    {
        private string currentFilePath;
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private string lastSavedText = ""; // Для отслеживания последнего сохраненного текста
        private bool isTextChanged = false; // Флаг для отслеживания изменений в тексте

        public Form1()
        {
            InitializeComponent();
        }
            

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripButton6.Click += toolStripButton6_Click;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли несохраненные изменения
            if (isTextChanged)
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в текущем файле?", "Сохранить изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile(); // Сохраняем текущий файл
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Отменяем открытие нового файла
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = File.ReadAllText(openFileDialog.FileName);
                    currentFilePath = openFileDialog.FileName; // Сохраняем путь к открытому файлу
                    lastSavedText = textBox.Text; // Сохраняем текст
                    isTextChanged = false; // Сбрасываем флаг изменений
                    undoStack.Clear(); // Очищаем стек отмены
                    redoStack.Clear(); // Очищаем стек повтора
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Проверяем, есть ли несохраненные изменения
            if (isTextChanged)
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в текущем файле?", "Сохранить изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile(); // Сохраняем текущий файл
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Отменяем закрытие формы
                }
            }

            base.OnFormClosing(e); // Вызываем базовый метод
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs(); // Если файл не был сохранен, используем "Сохранить как"
            }
            else
            {
                File.WriteAllText(currentFilePath, textBox.Text); // Сохраняем изменения в существующий файл
                lastSavedText = textBox.Text; // Обновляем последнее сохраненное состояние
                isTextChanged = false; // Сбрасываем флаг изменений
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs(); // Всегда открываем диалог "Сохранить как"
        }

        private void SaveFileAs()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, textBox.Text);
                    currentFilePath = saveFileDialog.FileName; // Сохраняем путь к новому файлу
                    lastSavedText = textBox.Text; // Сохраняем текст
                    isTextChanged = false; // Сбрасываем флаг изменений
                    undoStack.Clear(); // Очищаем стек отмены
                    redoStack.Clear(); // Очищаем стек повтора
                }
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли несохраненные изменения
            if (isTextChanged)
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в текущем файле?", "Сохранить изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile(); // Сохраняем текущий файл
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Отменяем создание нового файла
                }
            }

            // Очищаем текстовое поле и сбрасываем путь к файлу
            textBox.Clear();
            currentFilePath = string.Empty; // Сбрасываем путь к файлу
            lastSavedText = ""; // Сбрасываем последнее сохраненное состояние
            isTextChanged = false; // Сбрасываем флаг изменений
            undoStack.Clear(); // Очищаем стек отмены
            redoStack.Clear(); // Очищаем стек повтора
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) // Кнопка "Отмена"
        {
            Undo();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) // Кнопка "Повтор"
        {
            Redo();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, была ли нажата клавиша пробела или Enter
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                // Сохраняем текущее состояние текста в стек отмены
                undoStack.Push(textBox.Text);
                redoStack.Clear(); // Очищаем стек повтора при новом изменении
            }
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                // Сохраняем текущее состояние текста в стек повтора
                redoStack.Push(textBox.Text);
                // Восстанавливаем предыдущее состояние текста
                textBox.Text = undoStack.Pop();
                textBox.SelectionStart = textBox.Text.Length; // Устанавливаем курсор в конец текста
            }
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                // Сохраняем текущее состояние текста в стек отмены
                undoStack.Push(textBox.Text);
                // Восстанавливаем состояние текста из стека повтора
                textBox.Text = redoStack.Pop();
                textBox.SelectionStart = textBox.Text.Length; // Устанавливаем курсор в конец текста
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
                textBox.SelectedText = "";
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                textBox.Paste();
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText != "")
            {
                textBox.SelectedText = "";
                isTextChanged = true; // Устанавливаем флаг изменений
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            textBox.SelectAll();
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFilePath = "C:\\Users\\xxxma\\OneDrive\\Рабочий стол\\НЯМ НЯМ\\Компиляторы\\РГЗ\\help2.html"; // Убедитесь, что файл находится в корне проекта или укажите полный путь
            System.Diagnostics.Process.Start(helpFilePath);
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Текстовый редактор на C#\nВерсия 1.0\n\n" +
                    "Автор: Кляйншмидт Максим\n" +
                    "Описание: Это простой текстовый редактор с базовыми функциями редактирования текста.",
                    "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            Redo();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true;
        }
        private void AnalyzeButtonClick(object sender, EventArgs e)
        {
            //        (Регулярные выражения для поиска цитат)
            string inputText = textBox.Text;
            string pattern = "[«\"](.*?)[»\"]"; // фиксированный паттерн для поиска цитат

            try
            {
                List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

                if (matches.Count == 0)
                {
                    resultTextBox.Text = "Цитаты не найдены.";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Найденные цитаты:");

                foreach (var match in matches)
                {
                    sb.AppendLine(match.ToString());
                }

                resultTextBox.Text = sb.ToString();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultTextBox.Text = string.Empty;
            }
        }     

            /*          (Регулярные выражения для проверки слов, содержащих подстроку "кофе" и ровно 10 символов)
            string inputText = textBox.Text;

            // Регулярное выражение для поиска слов с "кофе" длиной ровно 10 символов
            string pattern = @"\b(?=\w{10}\b)\w*кофе\w*\b";

            try
            {
                List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

                if (matches.Count == 0)
                {
                    resultTextBox.Text = "Подходящих слов не найдено.";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Найденные слова:");

                foreach (var match in matches)
                {
                    sb.AppendLine(match.ToString());
                }

                resultTextBox.Text = sb.ToString();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultTextBox.Text = string.Empty;
            }
            */

        
            /*string inputText = textBox.Text;

            // Регулярное выражение для поиска времени в формате ЧЧ:ММ:СС с ведущими нулями
            string pattern = @"\b(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d\b";

            try
            {
                List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

                if (matches.Count == 0)
                {
                    resultTextBox.Text = "Время в заданном формате не найдено.";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Найденное время:");

                foreach (var match in matches)
                {
                    sb.AppendLine(match.ToString());
                }

                resultTextBox.Text = sb.ToString();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultTextBox.Text = string.Empty;
            }
          */ 


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли несохраненные изменения
            if (isTextChanged)
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в текущем файле?", "Сохранить изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile(); // Сохраняем текущий файл
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Отменяем создание нового файла
                }
            }

            // Очищаем текстовое поле и сбрасываем путь к файлу
            textBox.Clear();
            currentFilePath = string.Empty; // Сбрасываем путь к файлу
            lastSavedText = ""; // Сбрасываем последнее сохраненное состояние
            isTextChanged = false; // Сбрасываем флаг изменений
            undoStack.Clear(); // Очищаем стек отмены
            redoStack.Clear(); // Очищаем стек повтора

        }
       
       

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            Redo();
        }

        private void Сохранить_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли несохраненные изменения
            if (isTextChanged)
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения в текущем файле?", "Сохранить изменения", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveFile(); // Сохраняем текущий файл
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Отменяем открытие нового файла
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = File.ReadAllText(openFileDialog.FileName);
                    currentFilePath = openFileDialog.FileName; // Сохраняем путь к открытому файлу
                    lastSavedText = textBox.Text; // Сохраняем текст
                    isTextChanged = false; // Сбрасываем флаг изменений
                    undoStack.Clear(); // Очищаем стек отмены
                    redoStack.Clear(); // Очищаем стек повтора
                }
            }
        }

        private void Сохранить_как_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void Отменить_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Копировать_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
            }
        }

        private void Вырезать_Click(object sender, EventArgs e)
        {
            if (textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
                textBox.SelectedText = "";
            }
        }

        private void Вставить_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                textBox.Paste();
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            textBox.SelectAll();
            if (textBox.SelectedText != "")
            {
                textBox.SelectedText = "";
                isTextChanged = true; // Устанавливаем флаг изменений
            }
        }

        private void resultTextBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void пToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalyzeButtonClick(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string inputText = textBox.Text;
            string pattern = "[«\"](.*?)[»\"]"; // фиксированный паттерн для поиска цитат

            try
            {
                List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

                if (matches.Count == 0)
                {
                    resultTextBox.Text = "Цитаты не найдены.";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Найденные цитаты:");

                foreach (var match in matches)
                {
                    sb.AppendLine(match.ToString());
                }

                resultTextBox.Text = sb.ToString();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resultTextBox.Text = string.Empty;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //         (Регулярные выражения для проверки слов, содержащих подстроку "кофе" и ровно 10 символов)
string inputText = textBox.Text;

// Регулярное выражение для поиска слов с "кофе" длиной ровно 10 символов
string pattern = @"\b(?=\w{10}\b)\w*кофе\w*\b";

try
{
    List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

    if (matches.Count == 0)
    {
        resultTextBox.Text = "Подходящих слов не найдено.";
        return;
    }

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Найденные слова:");

    foreach (var match in matches)
    {
        sb.AppendLine(match.ToString());
    }

    resultTextBox.Text = sb.ToString();
}
catch (ArgumentException ex)
{
    MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    resultTextBox.Text = string.Empty;
}
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string inputText = textBox.Text;

// Регулярное выражение для поиска времени в формате ЧЧ:ММ:СС с ведущими нулями
string pattern = @"\b(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d\b";

try
{
    List<RegexSearcher.MatchInfo> matches = RegexSearcher.FindMatches(inputText, pattern);

    if (matches.Count == 0)
    {
        resultTextBox.Text = "Время в заданном формате не найдено.";
        return;
    }

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Найденное время:");

    foreach (var match in matches)
    {
        sb.AppendLine(match.ToString());
    }

    resultTextBox.Text = sb.ToString();
}
catch (ArgumentException ex)
{
    MessageBox.Show($"Ошибка в регулярном выражении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    resultTextBox.Text = string.Empty;
}
        }
    }
}