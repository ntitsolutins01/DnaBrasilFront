var vm = new Vue({
    el: "#vCursoDetalhes",
    data: {
        loading: false,
        selectedVideoUrl: "",
        editDto: { Id: "" },
        aula: null,
        aulas: []
    },
    mounted: function () {
        var self = this;
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
                var aulaAtualId = null;

                // Manipulação do clique nas aulas
                $('.aula-link').on('click', function (e) {
                    e.preventDefault();

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

                    // Atualizar os materiais
                    var materiaisJson = $(this).data('materiais');
                    atualizarMateriais(materiaisJson);

                    // Habilitar botão de próxima aula se não for a última
                    var $proximaAula = $(this).closest('li').next('li').find('.aula-link');
                    $('#btnProximaAula').prop('disabled', $proximaAula.length === 0);

                    // Armazenar ID da aula atual
                    aulaAtualId = $(this).data('aula-id');

                    // Habilitar botão de marcar como concluído
                    $('#btnMarcarConcluido').prop('disabled', false);
                });

                // Update de tempo assistido
                let aulas = [];
                let alreadyPosted = false; // Flag de controle
                const alunoId = document.getElementById('alunoId').value;

                function loadAulas() {
                    axios.get(`../../Aluno/GetAlunoAulasByAlunoId?alunoId=${alunoId}`)
                        .then(response => {
                            aulas = response.data || [];
                        })
                        .catch(() => {
                            aulas = [];
                        });
                }

                loadAulas();

                const video = document.getElementById('videoPlayer');

                video.addEventListener('timeupdate', function () {
                    if (!video.duration || alreadyPosted || !aulaAtualId) return;

                    const progress = (video.currentTime / video.duration) * 100;

                    if (progress >= 60) {
                        const exists = aulas.some(aula =>
                            aula.AlunoId == alunoId &&
                            aula.AulaId == aulaAtualId
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
                                    aulas.push({ AlunoId: alunoId, AulaId: aulaAtualId });
                                })
                                .catch(err => {
                                    alreadyPosted = false;
                                    console.error('Erro:', err);
                                });
                        }
                    }
                });


                // Função para atualizar a lista de materiais
                function atualizarMateriais(materiaisJson) {
                    // Limpar a lista atual
                    $('#lista-materiais').empty();

                    if (materiaisJson && materiaisJson.length > 0) {
                        try {
                            var materiais = JSON.parse(materiaisJson);

                            if (materiais.length > 0) {
                                materiais.forEach(function (material) {
                                    var icon = 'fa-file-o';
                                    var colorClass = '';

                                    // Definir ícone com base no tipo de arquivo
                                    if (material.url.endsWith('.pdf')) {
                                        icon = 'fa-file-pdf-o';
                                        colorClass = 'text-danger';
                                    } else if (material.url.endsWith('.pptx') || material.url.endsWith('.ppt')) {
                                        icon = 'fa-file-powerpoint-o';
                                        colorClass = 'text-warning';
                                    } else if (material.url.endsWith('.xlsx') || material.url.endsWith('.xls')) {
                                        icon = 'fa-file-excel-o';
                                        colorClass = 'text-success';
                                    } else if (material.url.endsWith('.docx') || material.url.endsWith('.doc')) {
                                        icon = 'fa-file-word-o';
                                        colorClass = 'text-primary';
                                    } else if (material.url.endsWith('.zip') || material.url.endsWith('.rar')) {
                                        icon = 'fa-file-archive-o';
                                        colorClass = 'text-warning';
                                    }

                                    var materialHtml = '<li class="list-group-item">' +
                                        '<i class="fa ' + icon + ' ' + colorClass + ' mr-xs"></i>' +
                                        '<a href="' + material.url + '" target="_blank">' + material.nome + '</a>' +
                                        '</li>';

                                    $('#lista-materiais').append(materialHtml);
                                });
                                return;
                            }
                        } catch (e) {
                            console.error("Erro ao processar materiais JSON:", e);
                        }
                    }

                    // Se não há materiais ou ocorreu um erro
                    $('#lista-materiais').append('<li class="list-group-item text-center"><i class="fa fa-info-circle"></i> Nenhum material disponível para esta aula.</li>');
                }

                // Botão para marcar aula como concluída
                $('#btnMarcarConcluido').on('click', function () {
                    if (!aulaAtualId) return;

                    // Simular uma chamada para o backend
                    marcarAulaComoAssistida(aulaAtualId);
                });

                // Botão de próxima aula
                $('#btnProximaAula').on('click', function () {
                    var $aulaAtual = $('.aula-link.active');
                    var $proximaAula = $aulaAtual.closest('li').next('li').find('.aula-link');

                    if ($proximaAula.length) {
                        $proximaAula.click();
                    }
                });

                // Função para marcar aula como assistida
                function marcarAulaComoAssistida(aulaId) {
                    // Simular uma chamada para o backend
                    console.log('Marcando aula ' + aulaId + ' como assistida');

                    // Para fins de demonstração:
                    atualizarAulaAssistida(aulaId);

                    // Calcular novo progresso (simulação)
                    var totalAulas = $('.aula-link').length;
                    var aulasAssistidas = $('.aula-link').find('.fa-check-circle').length + 1;
                    var novoProgresso = Math.round(aulasAssistidas / totalAulas);

                    atualizarProgresso(novoProgresso);

                    // Notificar o usuário
                    new PNotify({
                        title: 'Aula Concluída',
                        text: 'Seu progresso foi atualizado!',
                        type: 'success'
                    });
                }

                // Função para atualizar a interface quando uma aula é marcada como assistida
                function atualizarAulaAssistida(aulaId) {
                    var $aula = $('.aula-link[data-aula-id="' + aulaId + '"]');
                    if (!$aula.find('.fa-check-circle').length) {
                        $aula.append('<span class="pull-right text-success"><i class="fa fa-check-circle"></i></span>');
                    }
                }

                // Função para atualizar o progresso do curso
                function atualizarProgresso(progresso) {
                    $('.progress-bar').css('width', progresso + '%').attr('aria-valuenow', progresso).text(progresso + '%');

                    // Verificar se o curso foi concluído
                    if (progresso >= 100) {
                        // Habilitar o botão de certificado
                        var certificadoBtn = $('button.btn-default[disabled]').replaceWith(
                            '<a href="#" class="btn btn-success btn-block">' +
                            '<i class="fa fa-certificate"></i> Ver Certificado</a>'
                        );

                        // Notificar conclusão do curso
                        new PNotify({
                            title: 'Parabéns!',
                            text: 'Você concluiu o curso com sucesso! Agora você pode acessar seu certificado.',
                            type: 'success'
                        });
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

            $("#ddlEstado").change(function () {

                self.ShowLoad(true, "pFiltro");

                var sigla = $("#ddlEstado").val();

                var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

                var ddlSource = "#ddlMunicipio";

                $.getJSON(url,
                    { id: $(ddlSource).val() },
                    function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Municipio</option>';
                            $("#ddlMunicipio").empty;
                            $.each(data,
                                function (i, row) {
                                    items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                });
                            $("#ddlMunicipio").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Usuario',
                                text: data,
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
            });

            var formid = $('form')[0].id;

            if (formid === "formEditAlunoCertificado") {

                $("#formEditAlunoCertificado ").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }

            if (formid === "formAlunoCertificado") {

                $("#formAlunoCertificado").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }

            $("#ddlEstado").change(function () {

                self.ShowLoad(true, "pFiltro");

                var sigla = $("#ddlEstado").val();

                var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

                var ddlSource = "#ddlMunicipio";

                $.getJSON(url,
                    { id: $(ddlSource).val() },
                    function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Municipio</option>';
                            $("#ddlMunicipio").empty;
                            $.each(data,
                                function (i, row) {
                                    items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                });
                            $("#ddlMunicipio").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Usuario',
                                text: data,
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
            });

            //clique de escolha do select
            $("#ddlMunicipio").change(function () {

                self.ShowLoad(true, "pFiltro");

                var id = $("#ddlMunicipio").val();

                var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                var ddlSource = "#ddlLocalidade";

                $.getJSON(url,
                    { id: $(ddlSource).val() },
                    function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Localidade</option>';
                            $("#ddlLocalidade").empty;
                            $.each(data,
                                function (i, row) {
                                    items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                });
                            $("#ddlLocalidade").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Localidades',
                                text: 'Localidades não encontradas.',
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
            });

            $("#ddlLocalidade").change(function () {

                self.ShowLoad(true, "pFiltro");

                var id = $("#ddlLocalidade").val();

                var url = "../../Aluno/GetAlunosByLocalidadeId?id=" + id;

                var ddlSource = "#ddlAluno";

                $.getJSON(url,
                    { id: $(ddlSource).val() },
                    function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Aluno</option>';
                            $("#ddlAluno").empty;
                            $.each(data,
                                function (i, row) {
                                    items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                });
                            $("#ddlAluno").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Aluno',
                                text: 'Alunos não encontrados.',
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
            });

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
        selectLesson: function (id) {
            this.getAulaById(id);
        },
        getAulaById: function (id) {
            var self = this;
            axios.get("../../Aula/GetAulaById?id=" + id)
                .then(response => {
                    self.aula = response.data;
                    self.selectedVideoUrl = "";
                    if (response.data.video != undefined) {
                        self.selectedVideoUrl = "\\Aulas" + response.data.video.split("\\Aulas")[1];
                    }

                    $('#aulaAtualTitulo').text(response.data.titulo);

                    var player = document.getElementById("videoPlayer");
                    if (player) {
                        player.load();
                    }
                })
                .catch(error => {
                    console.error("Erro ao buscar aula:", error);
                });
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
    },
    AlunoCertificadoModal: function (id) {
        $('input[name="certificadoId"]').attr('value', id);
        $('#mdAlunoCertificado').modal('show');
        vm.AlunoCertificado(id);
    }
};