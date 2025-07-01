let currentPageNumber = 1;
const totalPages = document.querySelectorAll('.page-container').length;
const prevButton = document.getElementById('prevButton');
const nextButton = document.getElementById('nextButton');
document.getElementById('totalPages').textContent = totalPages;

function updateNavigationState() {
    // Desabilita o botão "anterior" se estiver na primeira página
    if (currentPageNumber <= 1) {
        prevButton.classList.add('disabled');
    } else {
        prevButton.classList.remove('disabled');
    }

    // Desabilita o botão "próximo" se estiver na última página
    if (currentPageNumber >= totalPages) {
        nextButton.classList.add('disabled');
    } else {
        nextButton.classList.remove('disabled');
    }

    document.getElementById('currentPage').textContent = currentPageNumber;
}

function previousPage() {
    if (currentPageNumber > 1) {
        currentPageNumber--;
        const prevPage = document.querySelectorAll('.page-container')[currentPageNumber - 1];
        prevPage.scrollIntoView({ behavior: 'smooth' });
        //updateNavigationState();
    }
}
function nextPage() {
    if (currentPageNumber < totalPages) {
        currentPageNumber++;
        const nextPage = document.querySelectorAll('.page-container')[currentPageNumber - 1];
        nextPage.scrollIntoView({ behavior: 'smooth' });
        //updateNavigationState();
    }
}

function printDocument() {
    window.print();
}
function downloadDocument() {
    console.log('Download iniciado');
    window.print();
}

// Observador de scroll para atualizar o número da página
const observerOptions = {
    root: null,
    threshold: 0.5,
};
const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (entry.isIntersecting) {
            const pages = document.querySelectorAll('.page-container');
            currentPageNumber = Array.from(pages).indexOf(entry.target) + 1;
            updateNavigationState();
        }
    });
}, observerOptions);
document.querySelectorAll('.page-container').forEach((page) => {
    observer.observe(page);
});

// Função para selecionar resposta (TEMPORARIA)
function selectAnswer(questionNumber, option) {
    // Remove seleção anterior da mesma questão
    const questionRows = document.querySelectorAll('.question-item');
    questionRows.forEach((row) => {
        const number = row.querySelector('.question-number').textContent;
        if (number == questionNumber) {
            const circles = row.querySelectorAll('.answer-circle');
            circles.forEach((circle) => circle.classList.remove('selected'));
        }
    });

    // Adiciona seleção ao círculo clicado
    event.target.classList.add('selected');

    let input = document.getElementById('resposta_' + questionNumber);
    if (input) {
        input.value = option;
    }
}
