var vm = new Vue({
    el: "#vDashboardEad",
    data: {
        loading: false
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';
            var formid = $('form')[1].id;

            if (formid === "formDashboardEad") {

                self.GetPesquisaDashboard();

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

                $select.on('change', function () {
                    $(this).trigger('blur');
                });

                $("#formDashboardEad").validate({
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

                $("#ddlFomento").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var id = $("#ddlFomento").val().split("-")[0];

                    var url = "../DivisaoAdministrativa/GetMunicipioByFomento?id=" + id;


                    $.getJSON(url,
                        function (data) {
                            if (data.length > 0) {
                                $("#ddlEstado").val(data[1]).trigger("change");
                                //var items = '<option value="">Selecionar Município</option>';
                                //$("#ddlMunicipio").empty;
                                //$.each(data,
                                //    function (i, row) {
                                //        if (row.selected === true) {
                                //            items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                //            $("#ddlMunicipio").val(row.value).trigger("change")
                                //        } else {
                                //            items += "<option value='" + row.value + "'>" + row.text + "</option>";

                                //        }
                                //    });
                                //$("#ddlMunicipio").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Municipios',
                                    text: 'Municipios não encontradas.',
                                    type: 'warning'
                                });
                            }
                        });

                    self.ShowLoad(false, "pFiltro");
                });

                $("#ddlEstado").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var sigla = $("#ddlEstado").val();
                    var municipioId = $("#ddlFomento").val().split("-")[1];

                    var url = "DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

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
                                $("#ddlMunicipio").val(municipioId).trigger("change");
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

                $("#ddlMunicipio").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var id = $("#ddlMunicipio").val();

                    var url = "../Localidade/GetLocalidadeByMunicipio?id=" + id;

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

                $("#ddlTipoCurso").change(function () {
                    var tipoCursoId = $("#ddlTipoCurso").val();
                    var url = "../Curso/GetCursosAllByTipoCursoId";

                    $.getJSON(url,
                        { id: tipoCursoId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Curso</option>';
                                $("#ddlCurso").empty();
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlCurso").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Curso',
                                    text: 'Cursos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });
            }
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
        GetPesquisaDashboard: function () {

            var self = this;
            self.ShowLoad(true, "pTipoCursosConcluidos");

            const obj = {
                FomentoId: $("#ddlFomento").val(),
                Estado: $("#ddlEstado").val(),
                MunicipioId: $("#ddlMunicipio").val(),
                LocalidadeId: $("#ddlLocalidade").val(),
                TipoCursoId: $("#ddlTipoCurso").val(),
                CursoId: $("#ddlCurso").val()
            }

            let axiosConfig = {
                headers: {
                    "Content-Type": 'application/json;charset=UTF-8',
                    "Access-Control-Allow-Origin": "*"
                }
            };

            //Busca indicadores de acordo com o filtro selecionado
            axios.post("DashboardEad/GetIndicadoresAlunosByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pIndicadores");

                $("#cadastrosMasculinos").text(result.data.dashboard.cadastrosMasculinos);
                $("#avaliacoesDna").text(result.data.dashboard.avaliacoesDna);
                $("#laudosAndamentos").text(result.data.dashboard.laudosAndamentos);
                $("#laudosFinalizados").text(result.data.dashboard.laudosFinalizados);
                $("#cadastrosFemininos").text(result.data.dashboard.cadastrosFemininos);
                $("#alunosCadastrados").text(result.data.dashboard.alunosCadastrados);
                $("#laudosMasculinos").text(result.data.dashboard.laudosMasculinos);
                $("#laudosFemininos").text(result.data.dashboard.laudosFemininos);

                self.ShowLoad(false, "pIndicadores");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pIndicadores");
            });

            monta grafico de Tipo de cursos concluidos por sexo
            axios.post("DashboardEad/GetGraficosSaudeByFilter", obj, axiosConfig).then(result => {
                var self = this;

            self.ShowLoad(true, "pTipoCursosConcluidos");

            self.SetGraficoTotalizadorTipoCursosConcluidos();// (result);

            self.ShowLoad(false, "pTipoCursosConcluidos");

            //}).catch(error => {
            //    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            //    self.ShowLoad(false, "pTipoCursosConcluidos");
            //});

            //monta grafico de notas por disciplina
            //axios.post("DashboardEad/GetGraficosSaudeByFilter", obj, axiosConfig).then(result => {
            //    var self = this;

            self.ShowLoad(true, "pNotasDisciplinas");

            self.SetGraficoNotasDisciplinas();// (result);

            self.ShowLoad(false, "pNotasDisciplinas");

            //}).catch(error => {
            //    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            //    self.ShowLoad(false, "pNotasDisciplinas");
            //});
        },
        SetGraficoTotalizadorTipoCursosConcluidos: function (result) {

            $(function () {

                Highcharts.chart('containerTipoCursosConcluidos', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Oficinas Profissionalizantes',
                            'Curso de Formação Inicial Continuada - Docentes',
                            'Curso de Formação Inicial Continuada - Dicentes',
                            'Pós-Graduação',
                            'Cursos Preparatórios',
                            'Reforço Escolar'],

                        labels: {
                            style: {
                                fontSize: '12px'
                            }
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Total',
                            style: {
                                fontSize: '12px'
                            }
                        },

                        labels: {
                            style: {
                                fontSize: '12px'
                            }
                        }
                    },
                    legend: {
                        reversed: true,
                        itemStyle: {
                            fontSize: '12px'
                        }
                    },
                    tooltip: {
                        style: {
                            fontSize: '12px'
                        }
                    },
                    plotOptions: {
                        series: {
                            stacking: 'normal',
                            dataLabels: {
                                enabled: true,
                                style: {
                                    fontSize: '12px',
                                    fontWeight: '400'
                                }
                            }
                        }
                    },
                    series: [{
                        name: 'Feminino',
                        color: '#EC407A',
                        data: [
                            12, 32, 45, 65, 45, 78
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
                            78, 98, 56, 15, 78, 45
                        ]
                    }]
                });

            });
        },
        SetGraficoNotasDisciplinas: function (result) {

            $(function () {
                Highcharts.chart('containerNotasDisciplinas', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Matemática', 'Língua Portuguesa', 'História', 'Geografia', 'Química', 'Física'],
                        crosshair: true,
                        accessibility: {
                            description: 'Disciplinas'
                        },
                        labels: {
                            style: {
                                fontSize: '12px'
                            }
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Total de Alunos',
                            style: {
                                fontSize: '12px'
                            }
                        }
                    },
                    legend: {
                        reversed: true,
                        itemStyle: {
                            fontSize: '12px'
                        }
                    },
                    tooltip: {
                        valueSuffix: ' alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0
                        },
                        series: {
                            dataLabels: {
                                enabled: true,
                                style: {
                                    fontSize: '12px',
                                    fontWeight: '400'
                                }
                            }
                        }
                    },
                    series: [
                        {
                            name: 'Acima da Média',
                            data: [387, 280, 129, 643, 540, 343],
                            color: '#3498db'
                        },
                        {
                            name: 'Abaixo da Média',
                            data: [453, 140, 100, 140, 195, 113],
                            color: '#cd6155'
                        }
                    ]
                });
            });
        }
    }
});

