var vm = new Vue({
    el: "#vControleFrequenciaEscolar",
    data: {
        loading: false,
        editDto: { Id: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            if (formid === "formPesquisarControleFrequenciaEscolar") {

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

                //clique de escolha do select
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
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Profissional/GetProfissionaisByLocalidade/" + id;
                    $.getJSON(url,
                        { id: id },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Profissional</option>';
                                $("#ddlProfissional").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlProfissional").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: "Profissionais não encontrados.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlEtapa").change(function () {

                    var etapaId = $("#ddlEtapa").val();
                    var localidadeId = $("#ddlLocalidade").val();

                    if (localidadeId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a localidade.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (etapaId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a etapa de ensino.',
                            type: 'warning'
                        });
                        return;
                    }

                    var url = "../../Serie/GetSeriesByLocalidadeIdEtapaId/";

                    axios.get(url, {
                        params: {
                            localidadeId: localidadeId,
                            etapaId: etapaId
                        }
                    }).then(result => {
                        if (result.data && result.data.length > 0) {
                            var items = '<option value="">Selecionar Série</option>';
                            $("#ddlSerie").empty();
                            $.each(result.data,
                                function (i, row) {
                                    if (row.selected) {
                                        items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                    } else {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    }
                                });
                            $("#ddlSerie").html(items);
                        } else {
                            new PNotify({
                                title: 'Aluno',
                                text: 'Séries não encontradas.',
                                type: 'warning'
                            });
                        }
                    }).catch(error => {
                        console.error('Erro ao carregar dados:', error);
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    }).finally(function () {
                        // sempre será executado
                    });;
                });

                //clique de escolha do select
                $("#ddlSerie").change(function () {

                    var serie = $("#ddlSerie").val();
                    var etapaId = $("#ddlEtapa").val();
                    var localidadeId = $("#ddlLocalidade").val();

                    if (localidadeId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a localidade.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (etapaId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a etapa de ensino.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (serie === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a série',
                            type: 'warning'
                        });
                        return;
                    }

                    var url = "../../Serie/GetTurmasByLocalidadeIdEtapaIdSerie/";

                    axios.get(url, {
                        params: {
                            localidadeId: localidadeId,
                            etapaId: etapaId,
                            serie: serie
                        }
                    }).then(result => {
                        if (result.data && result.data.length > 0) {
                            var items = '<option value="">Selecionar Turma</option>';
                            $("#ddlTurma").empty();
                            $.each(result.data,
                                function (i, row) {
                                    if (row.selected) {
                                        items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                    } else {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    }
                                });
                            $("#ddlTurma").html(items);
                        } else {
                            new PNotify({
                                title: 'Aluno',
                                text: 'Turmas não encontradas.',
                                type: 'warning'
                            });
                        }
                    }).catch(error => {
                        console.error('Erro ao carregar dados:', error);
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    }).finally(function () {
                        // sempre será executado
                    });
                });
            }

            // Pesquisa de Alunos para preencher a frequência
            $("#formPesquisarControleFrequenciaEscolar").submit(function (e) {
                e.preventDefault();

                var $form = $(this);
                var $btn = $form.find('button[type="submit"]');
                $btn.prop('disabled', true);

                if (typeof self.ShowLoad === "function") self.ShowLoad(true, "tabela-container");

                $.ajax({
                    url: $form.attr('action'),
                    type: $form.attr('method'),
                    data: $form.serialize(),
                    success: function (html) {
                        $("#tabela-container").html(html);
                        $("#tabela-container").show();

                        var $table = $("#datatable-default");
                        if ($table.length) {
                            $('html, body').animate({ scrollTop: $table.offset().top }, 300);
                        } else {
                            new PNotify({
                                title: 'Frequência Escolar',
                                text: 'Nenhum resultado encontrado.',
                                type: 'info'
                            });
                        }
                    },

                    error: function (xhr, status, error) {
                        new PNotify({
                            title: 'Frequência Escolar',
                            text: 'Não foi possível pesquisar. ' + (xhr.responseText || error),
                            type: 'error'
                        });
                    },
                    complete: function () {
                        $btn.prop('disabled', false);
                        if (typeof self.ShowLoad === "function") self.ShowLoad(false, "tabela-container");
                    }
                });
            });

            // Botão de Marcar aulas do dia atual
            $(document).on('click', '#btnMarcarDiaAtual', function () {
                $(".checkbox-dia-hoje:not(:disabled)").prop("checked", true);
            });

            // Botão de envio das frequências
            $(document).on('click', '#btnSalvarFrequencias', function () {
                var frequencias = [];
                var $btn = $(this);
                $btn.prop('disabled', true);

                var serieId = $("#ddlTurma").val();
                var disciplinaId = $("#ddlDisciplina").val();
                var profissionalId = $("#ddlProfissional").val();

                var dataAtual = new Date();
                var ano = dataAtual.getFullYear();
                var mes = dataAtual.getMonth() + 1;
                var diaHoje = dataAtual.getDate();

                $('#tabela-container input[type="checkbox"]').each(function () {
                    var $cb = $(this);
                    var dia = $cb.data('dia');
                    var alunoId = $cb.data('aluno-id');
                    var presente = $cb.is(':checked');

                    if (dia > diaHoje) return;

                    var dataFrequencia = ano + '-' +
                        String(mes).padStart(2, '0') + '-' +
                        String(dia).padStart(2, '0') + 'T00:00:00';

                    frequencias.push({
                        AlunoId: alunoId.toString(),
                        SerieId: serieId.toString(),
                        DisciplinaId: disciplinaId.toString(),
                        ProfissionalId: profissionalId.toString(),
                        Controle: presente ? 'P' : 'F',
                        DataFrequencia: dataFrequencia
                    });
                });

                if (frequencias.length === 0) {
                    new PNotify({
                        title: 'Frequência Escolar',
                        text: 'Nenhuma frequência selecionada!',
                        type: 'warning'
                    });
                    $btn.prop('disabled', false);
                    return;
                }

                self.frequenciaChunks = self.SplitArrayInChunks(frequencias, 200);
                self.btnSalvar = $btn;

                self.EnviarChunk(0);
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
        DeleteControleFrequenciaEscolar: function (id) {
            var url = "ControleFrequenciaEscolar/Delete/" + id;
            $("#deleteControleFrequenciaEscolarHref").prop("href", url);
        },
        EditControleFrequenciaEscolar: function (id) {
            var self = this;

            axios.get("ControleFrequenciaEscolar/GetControleFrequenciaEscolarById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Controle = result.data.controle;
                self.editDto.Data = result.data.data;
                self.editDto.Justificativa = result.data.justificativa;
                self.editDto.NomeAluno = result.data.nomeAluno;
                self.editDto.MunicipioEstado = result.data.municipioEstado;
                self.editDto.NomeLocalidade = result.data.nomeLocalidade;
                self.editDto.AlunoId = result.data.alunoId;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        SplitArrayInChunks: function (arr, chunkSize) {
            var result = [];
            for (var i = 0; i < arr.length; i += chunkSize) {
                result.push(arr.slice(i, i + chunkSize));
            }
            return result;
        },
        EnviarChunk: function (index) {
            var self = this;
            var chunks = self.frequenciaChunks;
            var $btn = self.btnSalvar;

            if (index >= chunks.length) {
                new PNotify({
                    title: 'Frequência Escolar',
                    text: 'Frequências salvas com sucesso!',
                    type: 'success'
                });
                if ($btn) $btn.prop('disabled', false);
                setTimeout(function () {
                    location.reload();
                }, 2000);
                return;
            }

            if (!chunks[index] || !chunks[index].length) {
                self.EnviarChunk(index + 1);
                return;
            }

            $.ajax({
                url: '/ControleFrequenciaEscolar/SalvarFrequencias',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(chunks[index]),
                success: function () {
                    self.EnviarChunk(index + 1);
                },
                error: function () {
                    new PNotify({
                        title: 'Frequência Escolar',
                        text: 'Erro ao salvar frequências (lote ' + (index + 1) + ').',
                        type: 'error'
                    });
                    if ($btn) $btn.prop('disabled', false);
                }
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteControleFrequenciaEscolarId"]').attr('value', id);
        $('#mdDeleteControleFrequenciaEscolar').modal('show');
        vm.DeleteControleFrequenciaEscolar(id)
    },
    EditModal: function (id) {
        $('input[name="editControleFrequenciaEscolarId"]').attr('value', id);
        $('#mdEditControleFrequenciaEscolar').modal('show');
        vm.EditControleFrequenciaEscolar(id)
    }
};