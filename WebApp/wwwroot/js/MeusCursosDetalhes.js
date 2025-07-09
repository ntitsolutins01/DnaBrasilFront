var vm = new Vue({
    el: "#vMeusCursosDetalhes",
    data: {
        progresso: 0,
        loading: false,
        selectedVideoUrl: "",
        selectedMaterialUrl: "",
        editDto: { Id: "" },
        aula: null,
        aulas: []
    },
    mounted: function () {
        const vm = this;
        vm.aulas = [];

        var aulaAtualId = null;
        var alreadyPosted = false;

        (function ($) {
            'use strict';

            var $select = $(".select2").select2({
                allowClear: true
            });

            $(".select2").each(function () {
                var $this = $(this),
                    opts = {};

                var pluginOptions = $this.data('plugin-options');
                if (pluginOptions)
                    opts = pluginOptions;

                $this.themePluginSelect2(opts);
            });

            /*
             * When you change the value the select via select2, it triggers
             * a 'change' event, but the jquery validation plugin
             * only re-validates on 'blur'*/

            $select.on('change', function () {
                $(this).trigger('blur');
            });

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            // Inicialização da página
            $(document).ready(function () {

                // Manipulação do clique nas aulas
                $('.aula-link').on('click', function (e) {
                    e.preventDefault();

                    alreadyPosted = false;

                    // Remover classe ativa de todas as aulas
                    $('.aula-link').removeClass('active');

                    // Adicionar classe ativa à aula clicada
                    $(this).addClass('active');

                    // Atualizar o player de vídeo
                    var videoUrl = $(this).data('video-url');
                    $('#videoSource').attr('src', videoUrl);
                    $('#videoPlayer')[0].load();

                    // Atualizar título da aula
                    var aulaTitulo = $(this).data('aula-titulo');
                    $('#aulaAtualTitulo').text(aulaTitulo);

                    // Habilitar botão de próxima aula se não for a última
                    var $proximaAula = $(this).closest('li').next('li').find('.aula-link');
                    $('#btnProximaAula').prop('disabled', $proximaAula.length === 0);

                    // Armazenar ID da aula atual
                    aulaAtualId = $(this).data('aula-id');

                    // Habilitar botão de marcar como concluído
                    $('#btnMarcarConcluido').prop('disabled', false);
                });

                // Update de tempo assistido
                const alunoId = document.getElementById('alunoId').value;

                function loadAulas() {
                    axios.get(`../../Aluno/GetAlunoAulasByAlunoId?alunoId=${alunoId}`)
                        .then(response => {
                            vm.aulas = Array.isArray(response.data) ? response.data : [];
                            vm.AtualizarProgresso();
                        })
                        .catch(() => {
                            vm.aulas = [];
                            vm.AtualizarProgresso();
                        });
                }

                loadAulas();

                const video = document.getElementById('videoPlayer');

                video.addEventListener('timeupdate', function () {
                    if (!video.duration || alreadyPosted || !aulaAtualId) return;

                    const progress = (video.currentTime / video.duration) * 100;

                    if (progress >= 60) {
                        const exists = vm.aulas.some(a =>
                            a.AlunoId === +alunoId &&
                            a.AulaId === aulaAtualId
                        );

                        if (!exists) {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const form = new URLSearchParams();
                            form.append('AlunoId', alunoId);
                            form.append('AulaId', aulaAtualId);
                            form.append('Progresso', progress.toFixed(2));
                            form.append('__RequestVerificationToken', token);

                            alreadyPosted = true;

                            axios.post('/Aluno/CreateAlunoAula', form, {
                                headers: {
                                    'Content-Type': 'application/x-www-form-urlencoded'
                                },
                                withCredentials: true
                            })
                                .then(() => {
                                    atualizarAulaAssistida(aulaAtualId);

                                    const totalAulas = document.querySelectorAll('.aula-link').length;
                                    const aulasConcluidas = document.querySelectorAll('.aula-link .fa-check-circle').length;
                                    const novoProgresso = totalAulas > 0 ? Math.round((aulasConcluidas / totalAulas) * 100) : 0;

                                    vm.progresso = novoProgresso;
                                    document.querySelector('.progress-bar').style.width = `${novoProgresso}%`;
                                    document.querySelector('.progress-bar').setAttribute('aria-valuenow', novoProgresso);
                                    document.querySelector('.progress-bar').textContent = `${novoProgresso}%`;

                                    const cursoId = document.getElementById('cursoId').value;
                                    const updateForm = new URLSearchParams();
                                    updateForm.append('AlunoId', alunoId);
                                    updateForm.append('CursoId', cursoId);
                                    updateForm.append('Progresso', novoProgresso);
                                    updateForm.append('__RequestVerificationToken', token);

                                    axios.post('/AlunoCursoCertificado/UpdateProgresso', updateForm, {
                                        headers: {
                                            'Content-Type': 'application/x-www-form-urlencoded'
                                        },
                                        withCredentials: true
                                    });

                                    vm.aulas.push({ AlunoId: +alunoId, AulaId: aulaAtualId });
                                })
                                .catch(err => {
                                    alreadyPosted = false;
                                    console.error('Erro:', err);
                                });
                        }
                    }
                });

                // Botão de próxima aula
                $('#btnProximaAula').on('click', function () {
                    var $aulaAtual = $('.aula-link.active');
                    var $proximaAula = $aulaAtual.closest('li').next('li').find('.aula-link');

                    if ($proximaAula.length) {
                        $proximaAula.click();
                    }
                });

                // Função para atualizar a interface quando uma aula é marcada como assistida
                function atualizarAulaAssistida(aulaId) {
                    var $aula = $('.aula-link[data-aula-id="' + aulaId + '"]');
                    if (!$aula.find('.fa-check-circle').length) {
                        $aula.append('<span class="pull-right text-success"><i class="fa fa-check-circle"></i></span>');
                    }
                }

                // Abrir o primeiro módulo por padrão
                $('#collapse1').addClass('in');

                // Tooltips
                if ($.isFunction($.fn['tooltip'])) {
                    $('[data-toggle=tooltip],[rel=tooltip]').tooltip({ container: 'body' });
                }

                $('.btn').tooltip({
                    container: 'body',
                    placement: 'top'
                });
            });

            this.progresso = parseFloat(document.getElementById('progresso').value) || 0;

        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            var self = this;

            self.isLoading = flag;
            $("#" + el).loadingOverlay({
                "startShowing": flag
            });
            self.loading = flag;

            if (!flag) {
                self.isLoading = flag;
                $("#" + el).removeClass("loading-overlay-showing");
                self.loading = flag;
            } else {
                self.isLoading = flag;
                $("#" + el).addClass("loading-overlay-showing");
                self.loading = flag;
            }
        },
        DeleteAlunoCertificado: function (id) {
            var url = "AlunoCertificado/Delete/" + id;
            $("#deleteAlunoCertificadoHref").prop("href", url);
        },
        EditAlunoCertificado: function (id) {
            var self = this;

            axios.get("AlunoCertificado/GetAlunoCertificadoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        AlunoCertificado: function (id) {
            var self = this;

            axios.get("AlunoCertificado/GetAlunoCertificadoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        SelectLesson: function (id) {
            this.GetAulaById(id);
        },
        GetAulaById: function (id) {
            var self = this;
            vm.AtualizarProgresso();
            axios.get("../../Aula/GetAulaById?id=" + id)
                .then(response => {
                    self.ListarMateriais();
                    self.aula = response.data;
                    self.ListarMateriais();
                    self.selectedVideoUrl = "";
                    if (response.data.video !== undefined) {
                        if (response.data.video.includes("\\Aulas")) {
                            self.selectedVideoUrl = "\\Aulas" + response.data.video.split("\\Aulas")[1];
                        } else if (response.data.video.includes("\\MaterialEAD")) {
                            self.selectedVideoUrl = "\\MaterialEAD" + response.data.video.split("\\MaterialEAD")[1];
                        }
                    }

                    if (response.data.material !== undefined) {
                        if (response.data.material.includes("\\Aulas")) {
                            self.selectedMaterialUrl = "\\Aulas" + response.data.material.split("\\Aulas")[1];
                        } else if (response.data.material.includes("\\MaterialEAD")) {
                            self.selectedMaterialUrl = "\\MaterialEAD" + response.data.material.split("\\MaterialEAD")[1];
                        }
                    }

                    $('#aulaAtualTitulo').text(response.data.titulo);

                    var player = document.getElementById("videoPlayer");
                    if (player) {
                        player.load();
                    }

                    self.ListarMateriais();

                })
                .catch(error => {
                    console.error("Erro ao buscar aula:", error);
                });
        },
        ListarMateriais: function () {
            $("#pdf-viewer").hide().empty();
            const lista = $('#lista-materiais');
            lista.empty();

            if (!this.aula || !this.aula.material) {
                lista.append('<li class="list-group-item text-center"><i class="fa fa-info-circle"></i> Nenhum material disponível para esta aula.</li>');
                return;
            }

            const material = this.aula.material;
            const nomeMaterial = this.aula.nomeMaterial;
            let icon = 'fa-file-o';
            let colorClass = '';

            icon = 'fa-file-pdf-o';
            colorClass = 'text-danger';

            const caminho = "\\MaterialEAD" + material.split("\\MaterialEAD")[1];
            const url = caminho.replace(/\\/g, "/");

            lista.append(`
                <li class="list-group-item">
                    <i class="fa ${icon} ${colorClass} mr-xs"></i>
                    <a href="#" class="visualizar-pdf" data-pdf-url="${url}">${nomeMaterial}</a>
                </li>
            `);

            lista.find('.visualizar-pdf').on('click', function (e) {
                e.preventDefault();
                const pdfUrl = $(this).data('pdf-url');
                vm.MostrarPdfNoViewer(pdfUrl);
            });
        },
        MostrarPdfNoViewer(pdfUrl) {
            $("#pdf-viewer").show();
            $("#pdf-viewer").empty();

            $("#pdf-viewer").html(`
                <div id="pdfjs-container" style="width:100%; min-height:600px; background:#eaeaea;">
                    <canvas id="pdf-canvas" style="width:100%;"></canvas>
                    <div id="pdf-controls" style="margin-top:10px; text-align:center;">
                        <button id="pdf-prev" class="btn btn-sm btn-light">Anterior</button>
                        <span id="pdf-page-info">Página <span id="pdf-page-num">1</span> de <span id="pdf-page-count">1</span></span>
                        <button id="pdf-next" class="btn btn-sm btn-light">Próxima</button>
                    </div>
                </div>
            `);

            if (!window['pdfjsLib']) {
                alert("pdfjsLib não encontrado. Verifique se incluiu o script correto do PDF.js em '/assets/vendor/pdfjs/build/pdf.js'!");
                return;
            }

            pdfjsLib.GlobalWorkerOptions.workerSrc = '/assets/vendor/pdfjs/build/pdf.worker.js';

            let pdfDoc = null,
                pageNum = 1,
                pageRendering = false,
                pageNumPending = null,
                scale = 1.2,
                canvas = document.getElementById('pdf-canvas'),
                ctx = canvas.getContext('2d');

            function renderPage(num) {
                pageRendering = true;
                pdfDoc.getPage(num).then(function (page) {
                    var viewport = page.getViewport({ scale: scale });
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;

                    var renderContext = {
                        canvasContext: ctx,
                        viewport: viewport
                    };
                    var renderTask = page.render(renderContext);

                    renderTask.promise.then(function () {
                        pageRendering = false;
                        if (pageNumPending !== null) {
                            renderPage(pageNumPending);
                            pageNumPending = null;
                        }
                    });
                });

                document.getElementById('pdf-page-num').textContent = num;
            }

            function queueRenderPage(num) {
                if (pageRendering) {
                    pageNumPending = num;
                } else {
                    renderPage(num);
                }
            }

            function onPrevPage() {
                if (pageNum <= 1) return;
                pageNum--;
                queueRenderPage(pageNum);
            }
            function onNextPage() {
                if (pageNum >= pdfDoc.numPages) return;
                pageNum++;
                queueRenderPage(pageNum);
            }

            pdfjsLib.getDocument(pdfUrl).promise.then(function (pdfDoc_) {
                pdfDoc = pdfDoc_;
                document.getElementById('pdf-page-count').textContent = pdfDoc.numPages;
                renderPage(pageNum);
            });

            document.getElementById('pdf-prev').addEventListener('click', onPrevPage);
            document.getElementById('pdf-next').addEventListener('click', onNextPage);
        },
        AtualizarProgresso() {
            const alunoId = document.getElementById('alunoId').value;
            const cursoId = document.getElementById('cursoId').value;

            const totalAulas = document.querySelectorAll('.aula-link').length;
            const aulasConcluidas = document.querySelectorAll('.aula-link .fa-check-circle').length;
            const novoProgresso = totalAulas > 0 ? Math.round((aulasConcluidas / totalAulas) * 100) : 0;

            vm.progresso = novoProgresso;
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteAlunoCertificadoId"]').attr('value', id);
        $('#mdDeleteAlunoCertificado').modal('show');
        vm.DeleteAlunoCertificado(id)
    },
    EditModal: function (id) {
        $('input[name="editAlunoCertificadoId"]').attr('value', id);
        $('#mdEditAlunoCertificado').modal('show');
        vm.EditAlunoCertificado(id)
    }
};