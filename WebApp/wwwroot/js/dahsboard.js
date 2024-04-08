var vm = new Vue({
    el: "#vDashboard",
    data: {
        loading: false
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';
            var formid = $('form')[1].id;

            if (formid === "formDashboard") {

                self.GetPesquisaDashboard();

                if ($.isFunction($.fn['datepicker'])) {

                    $(function () {
                        $('[data-plugin-datepicker]').each(function () {
                            var $this = $(this),
                                opts = {};

                            var pluginOptions = $this.data('plugin-options');
                            if (pluginOptions)
                                opts = pluginOptions;

                            $this.themePluginDatePicker(opts);
                        });
                    });

                }

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

                $("#formDashboard").validate({
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

                $("#ddlEstado").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var sigla = $("#ddlEstado").val();

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
        DeleteDashboard: function (id) {
            var url = "Dashboard/Delete/" + id;
            $("#deleteDashboardHref").prop("href", url);
        },
        GetPesquisaDashboard: function () {

            var self = this;
            self.ShowLoad(true, "pIndicadores");
            self.ShowLoad(true, "pControlePresenca");
            self.ShowLoad(true, "pLaudosPeriodo");
            self.ShowLoad(true, "pStatusLaudos");
            self.ShowLoad(true, "pEvolutivo");
            self.ShowLoad(true, "pSaudePerc");
            self.ShowLoad(true, "pSaudeTot");
            self.ShowLoad(true, "pEtniaPerc");
            self.ShowLoad(true, "pEtniaTot");
            self.ShowLoad(true, "pDeficienciaPerc");
            self.ShowLoad(true, "pDeficienciaTot");
            self.ShowLoad(true, "pSaudeBucalPerc");
            self.ShowLoad(true, "pSaudeBucalTot");
            self.ShowLoad(true, "pTalentoPerc");
            self.ShowLoad(true, "pTalentoTot");

            var obj = {
                FomentoId: $("#ddlFomento").val(),
                Estado: $("#ddlEstado").val(),
                MunicipioId: $("#ddlMunicipio").val(),
                LocalidadeId: $("#ddlLocalidade").val(),
                DeficienciaId: $("#ddlDeficiencia").val(),
                Etnia: $("#ddlEtnia").val()
            }

            let axiosConfig = {
                headers: {
                    "Content-Type": 'application/json;charset=UTF-8',
                    "Access-Control-Allow-Origin": "*"
                }
            };

            axios.post("Dashboard/GetIndicadoresAlunosByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pIndicadores");

                $("#cadastrosMasculinos").text(result.data.dashboard.cadastrosMasculinos);
                $("#avaliacoesDna").text(result.data.dashboard.avaliacoesDna);
                $("#laudosAndamentos").text(result.data.dashboard.laudosAndamentos);
                $("#laudosFinalizados").text(result.data.dashboard.laudosFinalizados);
                $("#cadastrosFemininos").text(result.data.dashboard.cadastrosFemininos);
                $("#alunosCadastrados").text(result.data.dashboard.alunosCadastrados);
                $("#laudosMasculinos").text(result.data.dashboard.laudosMasculinos);
                $("#laudosMasculinos").text(result.data.dashboard.laudosMasculinos);

                self.ShowLoad(false, "pIndicadores");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pIndicadores");
            });
            axios.post("Dashboard/GetControlePresencaByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pControlePresenca");

                self.SetGraficoControlePresenca(result);

                self.ShowLoad(false, "pControlePresenca");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pControlePresenca");
            });
            axios.post("Dashboard/GetLaudosPeriodoByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pLaudosPeriodo");

                self.SetGraficoLaudosPeriodo(result);

                self.ShowLoad(false, "pLaudosPeriodo");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pLaudosPeriodo");
            });
            axios.post("Dashboard/GetStatusLaudosByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pStatusLaudos");

                $("#totSaudeFinalizado").text(result.data.dashboard.statusLaudos.totSaudeFinalizado);
                $("#totSaudeAndamento").text(result.data.dashboard.statusLaudos.totSaudeAndamento);
                var percentSaude = result.data.dashboard.statusLaudos.progressoSaude + '%'
                $('#progressoSaude').css('width', percentSaude);
                $('#vlProgressoSaude').text(result.data.dashboard.statusLaudos.progressoSaude + ' %');

                $("#totTalentoEsportivoFinalizado").text(result.data.dashboard.statusLaudos.totTalentoEsportivoFinalizado);
                $("#totTalentoEsportivoAndamento").text(result.data.dashboard.statusLaudos.totTalentoEsportivoAndamento);
                var percentTalentoEsportivo = result.data.dashboard.statusLaudos.progressoTalentoEsportivo + '%'
                $('#progressoTalentoEsportivo').css('width', percentTalentoEsportivo);
                $('#vlProgressoTalentoEsportivo').text(result.data.dashboard.statusLaudos.progressoTalentoEsportivo + ' %');

                $("#totConsumoAlimentarFinalizado").text(result.data.dashboard.statusLaudos.totConsumoAlimentarFinalizado);
                $("#totConsumoAlimentarAndamento").text(result.data.dashboard.statusLaudos.totConsumoAlimentarAndamento);
                var percentConsumoAlimentar = result.data.dashboard.statusLaudos.progressoConsumoAlimentar + '%'
                $('#progressoConsumoAlimentar').css('width', percentConsumoAlimentar);
                $('#vlProgressoConsumoAlimentar').text(result.data.dashboard.statusLaudos.vlProgressoConsumoAlimentar + ' %');

                $("#totSaudeBucalFinalizado").text(result.data.dashboard.statusLaudos.totSaudeBucalFinalizado);
                $("#totSaudeBucalAndamento").text(result.data.dashboard.statusLaudos.totSaudeBucalAndamento);
                var percentSaudeBucal = result.data.dashboard.statusLaudos.progressoSaudeBucal + '%'
                $('#progressoSaudeBucal').css('width', percentSaudeBucal);
                $('#vlProgressoSaudeBucal').text(result.data.dashboard.statusLaudos.vlProgressoSaudeBucal + ' %');

                $("#totSaudeBucalFinalizado").text(result.data.dashboard.statusLaudos.totSaudeBucalFinalizado);
                $("#totSaudeBucalAndamento").text(result.data.dashboard.statusLaudos.totSaudeBucalAndamento);
                var percentSaudeBucal = result.data.dashboard.statusLaudos.progressoSaudeBucal + '%'
                $('#progressoSaudeBucal').css('width', percentSaudeBucal);
                $('#vlProgressoSaudeBucal').text(result.data.dashboard.statusLaudos.vlProgressoSaudeBucal + ' %');

                $("#totQualidadeDeVidaFinalizado").text(result.data.dashboard.statusLaudos.totQualidadeDeVidaFinalizado);
                $("#totQualidadeDeVidaAndamento").text(result.data.dashboard.statusLaudos.totQualidadeDeVidaAndamento);
                var percentQualidadeDeVida = result.data.dashboard.statusLaudos.progressoQualidadeDeVida + '%'
                $('#progressoQualidadeDeVida').css('width', percentQualidadeDeVida);
                $('#vlProgressoQualidadeDeVida').text(result.data.dashboard.statusLaudos.progressoQualidadeDeVida + ' %');

                $("#totVocacionalFinalizado").text(result.data.dashboard.statusLaudos.totVocacionalFinalizado);
                $("#totVocacionalAndamento").text(result.data.dashboard.statusLaudos.totVocacionalAndamento);
                var percentVocacional = result.data.dashboard.statusLaudos.progressoVocacional + '%'
                $('#progressoVocacional').css('width', percentVocacional);
                $('#vlProgressoVocacional').text(result.data.dashboard.statusLaudos.progressoVocacional + ' %');


                self.ShowLoad(false, "pStatusLaudos");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pStatusLaudos");
            });

            axios.post("Dashboard/GetEvolutivoByFilter", obj, axiosConfig).then(result => {
                var self = this;
                self.ShowLoad(true, "pEvolutivo");

                self.SetGraficoEvolutivo(result);

                self.ShowLoad(false, "pEvolutivo");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pEvolutivo");
            });

            axios.post("Dashboard/GetGraficosSaudeByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pSaudePerc");
                self.ShowLoad(true, "pSaudeTot");

                self.SetGraficoSaudePercentual(result);
                self.SetGraficoTotalizadorSaudeSexo(result);

                self.ShowLoad(false, "pSaudePerc");
                self.ShowLoad(false, "pSaudeTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                self.ShowLoad(false, "pSaudePerc");
                self.ShowLoad(false, "pSaudeTot");
            });

            axios.post("Dashboard/GetGraficosEtniaByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pEtniaPerc");
                self.ShowLoad(true, "pEtniaTot");

                self.SetGraficoEtniaPercentual(result);
                self.SetGraficoTotalizadorEtnia(result);

                self.ShowLoad(false, "pEtniaPerc");
                self.ShowLoad(false, "pEtniaTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                self.ShowLoad(false, "pEtniaPerc");
                self.ShowLoad(false, "pEtniaTot");
            });

            axios.post("Dashboard/GetGraficosDeficienciasByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pDeficienciaPerc");
                self.ShowLoad(true, "pDeficienciaTot");

                self.SetGraficoDeficienciaPercentual(result);
                self.SetGraficoTotalizadorDeficiencia(result);

                self.ShowLoad(false, "pDeficienciaPerc");
                self.ShowLoad(false, "pDeficienciaTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                self.ShowLoad(false, "pDeficienciaPerc");
                self.ShowLoad(false, "pDeficienciaTot");
            });

            axios.post("Dashboard/GetGraficosSaudeBucalByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pSaudeBucalPerc");
                self.ShowLoad(true, "pSaudeBucalTot");

                self.SetGraficoSaudePercentual(result);
                self.SetGraficoTotalizadorSaudeSexo(result);

                self.ShowLoad(false, "pSaudeBucalPerc");
                self.ShowLoad(false, "pSaudeBucalTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                self.ShowLoad(false, "pSaudeBucalPerc");
                self.ShowLoad(false, "pSaudeBucalTot");
            });

            axios.post("Dashboard/GetGraficosTalentoByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pTalentoPerc");
                self.ShowLoad(true, "pTalentoTot");

                self.SetGraficoTalentoPercentual(result);
                self.SetGraficoTotalizadorTalento(result);

                self.ShowLoad(false, "pTalentoPerc");
                self.ShowLoad(false, "pTalentoTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                self.ShowLoad(false, "pTalentoPerc");
                self.ShowLoad(false, "pTalentoTot");
            });
            axios.post("Dashboard/GetGraficosTalentoByFilter", obj, axiosConfig).then(result => {

                self.SetGraficoPercDesempenhoFisicoMotor(result);
                self.SetGraficoTotDesempenhoFisicoMotor(result);

                self.SetGraficoQualidadePercentual(result);
                self.SetGraficoTotalizadorQualidade(result);

                self.SetGraficoConsumoPercentual(result);
                self.SetGraficoTotalizadorConsumo(result);

                self.SetGraficoBuscalPercentual(result);
                self.SetGraficoTotalizadorBucal(result);

                self.SetGraficoVocacionalPercentual(result);
                self.SetGraficoTotalizadorVocacional(result);

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        SetGraficoControlePresenca: function (result) {

            var self = this;

            $(function () {

                Highcharts.chart('containerPresenca', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Jav', 'Fev', 'Mar',
                            'Abr', 'Mai', 'Jun',
                            'Jul', 'Ago', 'Set',
                            'Out', 'Nov', 'Dez'],
                        labels: {
                            style: {
                                fontSize: '15px'
                            }
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Quantidade de Presenças e Faltas',
                            style: {
                                fontSize: '10px'
                            }
                        },
                        labels: {
                            style: {
                                fontSize: '15px'
                            }
                        }
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0
                        }
                    },
                    tooltip: {
                        style: {
                            fontSize: '15px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '15px'
                        }
                    },
                    colors: ['#4CAF50', '#F44336'],
                    series: [
                        {
                            name: 'Presença',
                            data: result.data.dashboard.listPresencasAnual
                        },
                        {
                            name: 'Falta',
                            data: result.data.dashboard.listFaltasAnual
                        }
                    ]
                });
            });
        },
        SetGraficoEvolutivo: function (result) {

            Highcharts.chart('containerEvolutivo', {
                xAxis: {
                    categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                },
                title: {
                    text: undefined
                },

                yAxis: {
                    title: {
                        text: undefined
                    }
                },

                series: [{
                    name: 'Velocidade',
                    data: [29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4],
                    color: '#DAF7A6'
                }, {
                    name: 'Flexibilidade muscular',
                    data: [24916, 37941, 29742, 29851, 32490, 30282,
                        38121, 36885, 33726, 34243, 31050],
                    color: '#FFC300'
                }, {
                    name: 'Forca de membro superiores',
                    data: [11744, 30000, 16005, 19771, 20185, 24377,
                        32147, 30912, 29243, 29213, 25663],
                    color: '#FF5733'
                }, {
                    name: 'Agilidade',
                    data: [null, null, null, null, null, null, null,
                        null, 11164, 11218, 10077],
                    color: '#900C3F'
                }, {
                    name: 'Resistencia abdominal',
                    data: [21908, 5548, 8105, 11248, 8989, 11816, 18274,
                        17300, 13053, 11906, 10073],
                    color: '#581845'
                }]
            });
        },
        SetGraficoLaudosPeriodo: function (result) {
            $(function () {

                Highcharts.chart('containerLaudos', {

                    chart: {
                        type: 'column',
                        inverted: false
                    },

                    title: {
                        text: undefined
                    },

                    xAxis: {
                        categories: ['Últimos 3 meses', 'Últimos 6 meses', 'Em 1 ano'],
                        labels: {
                            style: {
                                fontSize: '15px'
                            }
                        }
                    },

                    yAxis: {
                        title: {
                            text: 'Laudos',
                            style: {
                                fontSize: '10px'
                            },
                            labels: {
                                style: {
                                    fontSize: '15px'
                                }
                            }
                        }
                    },

                    plotOptions: {
                        columnrange: {
                            dataLabels: {
                                enabled: true,
                                formatter: function () {
                                    return this.y;
                                }
                            }
                        }
                    },
                    tooltip: {
                        style: {
                            fontSize: '15px'
                        }
                    },

                    legend: {
                        itemStyle: {
                            fontSize: '15px'
                        },
                        enabled: false
                    },

                    series: [{
                        name: 'Total',
                        data: [
                            { y: result.data.dashboard.ultimos3Meses, color: '#3949AB' },
                            { y: result.data.dashboard.ultimos6Meses, color: '#8E24AA' },
                            { y: result.data.dashboard.em1Ano, color: '#1E88E5' }
                        ]
                    }]

                });

            });
        },
        SetGraficoSaudePercentual: function (result) {
            $(function () {

                Highcharts.chart('containerSaudePercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de Saúde dos Alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'ABAIXO DO NORMAL',
                            y: result.data.dashboard.percentualSaude.ABAIXODONORMAL,
                            z: 50
                        }, {
                            name: 'NORMAL',
                            y: result.data.dashboard.percentualSaude.NORMAL,
                            z: 50
                        }, {
                            name: 'SOBREPESO',
                            y: result.data.dashboard.percentualSaude.SOBREPESO,
                            z: 50
                        }, {
                            name: 'OBESIDADE',
                            y: result.data.dashboard.percentualSaude.OBESIDADE,
                            z: 50
                        }],
                        colors: [
                            '#EF5350',
                            '#EC407A',
                            '#AB47BC',
                            '#7E57C2'
                        ]
                    }]
                });

            });
        },
        SetGraficoTotalizadorSaudeSexo: function (result) {

            $(function () {

                Highcharts.chart('containerSaudeSexo', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Baixo Peso', 'Acima do Peso', 'Risco de Colesterol Alto e Diabetes',
                            'Risco de Hipertensão Arterial e Transtornos Cardíacos', 'Pré – disposição a resistência a insulina',
                            'Pré – disposição a desequilíbrios musculares', 'Índice positivo de saúde'],

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
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.baixoPeso,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.acimaPeso,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.desequilibrioMuscular,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.indicePositivoSaude
                        ]
                    }, {
                        name: 'Masculino',
                        data: [

                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.baixoPeso,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.acimaPeso,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoColesterolAlto,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoHipertensao,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.resistenciaInsulina,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.desequilibrioMuscular,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.indicePositivoSaude
                        ]
                    }]
                });

            });
        },
        SetGraficoTalentoPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerTalentoPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de Saúde dos Alunos',
                        borderRadius: 5,
                        data: result.data.dashboard.listPercTalento
                    }]
                });

            });
        },
        SetGraficoTotalizadorTalento: function (result) {
            $(function () {

                Highcharts.chart('containerTalento', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: result.data.dashboard.listPercTalentoCategorias,

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
                        data: result.data.dashboard.listValorTalentoFem
                    }, {
                        name: 'Masculino',
                        data: result.data.dashboard.listValorTalentoMasc
                    }]
                });

            });
        },
        SetGraficoPercDesempenhoFisicoMotor: function (result) {
            $(function () {

                Highcharts.chart('containerPercDesempenhoFisicoMotor', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de Saúde dos Alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'Velocidade',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.velocidade,
                            z: 50
                        }, {
                            name: 'Flexibilidade Muscular',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.flexibilidadeMuscular,
                            z: 50
                        }, {
                            name: 'Força de Membros Superiores',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.forcaMembrosSup,
                            z: 50
                        }, {
                            name: 'Força Explosiva de Membros Inferiores',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.forcaExplosiva,
                            z: 50
                        }, {
                            name: 'Aptidão Cardiorrespiratória',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.aptidaoCardio,
                            z: 50
                        }, {
                            name: 'Agilidade',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.agilidade,
                            z: 50
                        }, {
                            name: 'Agilidade ou Shuttle run',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.shutlleRun,
                            z: 50
                        }, {
                            name: 'Resistência Abdominal',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.resAbdominal,
                            z: 50
                        }, {
                            name: 'Prancha (ABD)',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.prancha,
                            z: 50
                        }, {
                            name: 'Vo2 Max',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.vo2Max,
                            z: 50
                        }]
                    }]
                });

            });
        },
        SetGraficoTotDesempenhoFisicoMotor: function (result) {
            $(function () {

                Highcharts.chart('containerTotDesempenhoFisicoMotor', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Velocidade', 'Flexibilidade Muscular', 'Força de Membros Superiores', 'Força Explosiva de Membros Inferiores', 'Aptidão Cardiorrespiratória', 'Agilidade', 'Agilidade ou Shuttle run', 'Resistência Abdominal', 'Prancha (ABD)', 'Vo2 Max'],

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
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.velocidade,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.flexibilidadeMuscular,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.forcaMembrosSup,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.forcaExplosiva,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.aptidaoCardio,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.agilidade,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.shutlleRun,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.resAbdominal,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.prancha,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.vo2Max
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.velocidade,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.flexibilidadeMuscular,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.forcaMembrosSup,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.forcaExplosiva,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.aptidaoCardio,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.agilidade,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.shutlleRun,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.resAbdominal,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.prancha,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.vo2Max
                        ]
                    }]
                });

            });
        },
        SetGraficoDeficienciaPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerDeficienciaPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de Deficiência dos Alunos',
                        borderRadius: 5,
                        data: result.data.dashboard.listPercDeficiencia
                    }]
                });

            });
        },
        SetGraficoTotalizadorDeficiencia: function (result) {
            $(function () {

                Highcharts.chart('containerDeficiencia', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: result.data.dashboard.listPercDeficienciaCategorias,

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
                        data: result.data.dashboard.listValorDeficienciaFem
                    }, {
                        name: 'Masculino',
                        data: result.data.dashboard.listValorDeficienciaMasc
                    }]
                });

            });
        },
        SetGraficoEtniaPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerEtniaPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de Etnia dos Alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'PARDO',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PARDO,
                            z: 50
                        }, {
                            name: 'BRANCO',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.BRANCO,
                            z: 50
                        }, {
                            name: 'PRETO',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PRETO,
                            z: 50
                        }, {
                            name: 'INDÍGENA',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDÍGENA,
                            z: 50
                        }, {
                            name: 'AMARELO',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.AMARELO,
                            z: 50
                        }]
                    }]
                });

            });
        },
        SetGraficoTotalizadorEtnia: function (result) {
            $(function () {

                Highcharts.chart('containerEtnia', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['PARDO', 'BRANCO', 'PRETO',
                            'INDÍGENA', 'AMARELO'],

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
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.PARDO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.BRANCO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.PRETO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.INDÍGENA,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.AMARELO,
                        ]
                    }, {
                        name: 'Masculino',
                        data: [

                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.PARDO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.BRANCO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.PRETO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.INDÍGENA,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.AMARELO,
                        ]
                    }]
                });

            });
        },
        SetGraficoQualidadePercentual: function (result) {
            $(function () {

                Highcharts.chart('containerQualidadePercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de qualidade de vida dos alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'Bem estar físico',
                            y: 5,
                            z: 50
                        }, {
                            name: 'Mal estar físico',
                            y: 20,
                            z: 50
                        }, {
                            name: 'Autoestima e estabilidade emocional',
                            y: 20,
                            z: 50
                        }, {
                            name: 'Baixa autoestima e dificuldades emocionais',
                            y: 30,
                            z: 50
                        }, {
                            name: 'Funcionamento harmônico familiar',
                            y: 10,
                            z: 50
                        }, {
                            name: 'Conflitos no contexto familiar',
                            y: 15,
                            z: 50
                        }, {
                            name: 'Contextos favorecedores do desenvolvimento ',
                            y: 15,
                            z: 50
                        }, {
                            name: 'Contextos não favorecedores do desenvolvimento ',
                            y: 15,
                            z: 50
                        }]
                    }]
                });

                //Highcharts.chart('containerEtniaPercentual', {
                //    chart: {
                //        type: 'variablepie'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    tooltip: {
                //        headerFormat: '',
                //        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                //            '<b>{point.y} %</b> dos Alunos',
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    legend: {
                //        itemStyle: {
                //            fontSize: '2px'
                //        }
                //    },
                //    plotOptions: {
                //        variablepie: {
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        minPointSize: 10,
                //        innerSize: '20%',
                //        zMin: 0,
                //        name: 'Percentual de Etnia dos Alunos',
                //        borderRadius: 5,
                //        data: [{
                //            name: 'PARDO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PARDO,
                //            z: 50
                //        }, {
                //            name: 'BRANCO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.BRANCO,
                //            z: 50
                //        }, {
                //            name: 'PRETO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PRETO,
                //            z: 50
                //        }, {
                //            name: 'INDÍGENA',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDÍGENA,
                //            z: 50
                //        }, {
                //            name: 'AMARELO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.AMARELO,
                //            z: 50
                //        }],
                //        colors: [
                //            '#EF5350',
                //            '#EC407A',
                //            '#AB47BC',
                //            '#7E57C2'
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoTotalizadorQualidade: function (result) {
            $(function () {

                Highcharts.chart('containerQualidade', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Bem estar físico', 'Mal estar físico', 'Autoestima e estabilidade emocional', 'Baixa autoestima e dificuldades emocionais', 'Funcionamento harmônico familiar',
                            'Conflitos no contexto familiar', 'Contextos favorecedores do desenvolvimento ', 'Contextos não favorecedores do desenvolvimento ']
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Total'
                        }
                    },
                    legend: {
                        reversed: true
                    },
                    plotOptions: {
                        series: {
                            stacking: 'normal',
                            dataLabels: {
                                enabled: true
                            }
                        }
                    },
                    series: [{
                        name: 'Feminino',
                        color: '#EC407A',
                        data: [1, 0, 3, 0, 1, 2, 6, 3]
                    }, {
                        name: 'Masculino',
                        data: [1, 1, 0, 2, 2, 4, 3, 6]
                    }]
                });

                //Highcharts.chart('containerEtnia', {
                //    chart: {
                //        type: 'bar'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    xAxis: {
                //        categories: ['PARDO', 'BRANCO', 'PRETO',
                //            'INDÍGENA', 'AMARELO'],

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    yAxis: {
                //        min: 0,
                //        title: {
                //            text: 'Total',
                //            style: {
                //                fontSize: '12px'
                //            }
                //        },

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    legend: {
                //        reversed: true,
                //        itemStyle: {
                //            fontSize: '12px'
                //        }
                //    },
                //    tooltip: {
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    plotOptions: {
                //        series: {
                //            stacking: 'normal',
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Feminino',
                //        color: '#EC407A',
                //        data: [
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                //        ]
                //    }, {
                //        name: 'Masculino',
                //        data: [

                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.resistenciaInsulina,
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoConsumoPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerConsumoPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de consumo alimentar dos alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'Hábitos não saudáveis',
                            y: 30,
                            z: 50
                        }, {
                            name: 'Hábitos satisfatórios',
                            y: 20,
                            z: 50
                        }, {
                            name: 'Bons Hábitos alimentares',
                            y: 50,
                            z: 50
                        }, {
                            name: 'Hábitos Saudáveis',
                            y: 50,
                            z: 50
                        }]
                    }]
                });

                //Highcharts.chart('containerEtniaPercentual', {
                //    chart: {
                //        type: 'variablepie'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    tooltip: {
                //        headerFormat: '',
                //        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                //            '<b>{point.y} %</b> dos Alunos',
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    legend: {
                //        itemStyle: {
                //            fontSize: '2px'
                //        }
                //    },
                //    plotOptions: {
                //        variablepie: {
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        minPointSize: 10,
                //        innerSize: '20%',
                //        zMin: 0,
                //        name: 'Percentual de Etnia dos Alunos',
                //        borderRadius: 5,
                //        data: [{
                //            name: 'PARDO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PARDO,
                //            z: 50
                //        }, {
                //            name: 'BRANCO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.BRANCO,
                //            z: 50
                //        }, {
                //            name: 'PRETO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PRETO,
                //            z: 50
                //        }, {
                //            name: 'INDÍGENA',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDÍGENA,
                //            z: 50
                //        }, {
                //            name: 'AMARELO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.AMARELO,
                //            z: 50
                //        }],
                //        colors: [
                //            '#EF5350',
                //            '#EC407A',
                //            '#AB47BC',
                //            '#7E57C2'
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoTotalizadorConsumo: function (result) {
            $(function () {

                Highcharts.chart('containerConsumo', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Hábitos não saudáveis', 'Hábitos satisfatórios', 'Bons Hábitos alimentares', 'Hábitos Saudáveis'],

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
                        data: [1, 0, 3, 0]
                    }, {
                        name: 'Masculino',
                        data: [1, 1, 0, 2]
                    }]
                });

                //Highcharts.chart('containerEtnia', {
                //    chart: {
                //        type: 'bar'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    xAxis: {
                //        categories: ['PARDO', 'BRANCO', 'PRETO',
                //            'INDÍGENA', 'AMARELO'],

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    yAxis: {
                //        min: 0,
                //        title: {
                //            text: 'Total',
                //            style: {
                //                fontSize: '12px'
                //            }
                //        },

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    legend: {
                //        reversed: true,
                //        itemStyle: {
                //            fontSize: '12px'
                //        }
                //    },
                //    tooltip: {
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    plotOptions: {
                //        series: {
                //            stacking: 'normal',
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Feminino',
                //        color: '#EC407A',
                //        data: [
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                //        ]
                //    }, {
                //        name: 'Masculino',
                //        data: [

                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.resistenciaInsulina,
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoBuscalPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerBucalPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de consumo alimentar dos alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'CUIDADO',
                            y: 75,
                            z: 50
                        }, {
                            name: 'ATENÇÃO',
                            y: 0,
                            z: 50
                        }, {
                            name: 'MUITO BOM',
                            y: 25,
                            z: 50
                        }]
                    }]
                });

                //Highcharts.chart('containerEtniaPercentual', {
                //    chart: {
                //        type: 'variablepie'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    tooltip: {
                //        headerFormat: '',
                //        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                //            '<b>{point.y} %</b> dos Alunos',
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    legend: {
                //        itemStyle: {
                //            fontSize: '2px'
                //        }
                //    },
                //    plotOptions: {
                //        variablepie: {
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        minPointSize: 10,
                //        innerSize: '20%',
                //        zMin: 0,
                //        name: 'Percentual de Etnia dos Alunos',
                //        borderRadius: 5,
                //        data: [{
                //            name: 'PARDO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PARDO,
                //            z: 50
                //        }, {
                //            name: 'BRANCO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.BRANCO,
                //            z: 50
                //        }, {
                //            name: 'PRETO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PRETO,
                //            z: 50
                //        }, {
                //            name: 'INDÍGENA',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDÍGENA,
                //            z: 50
                //        }, {
                //            name: 'AMARELO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.AMARELO,
                //            z: 50
                //        }],
                //        colors: [
                //            '#EF5350',
                //            '#EC407A',
                //            '#AB47BC',
                //            '#7E57C2'
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoTotalizadorBucal: function (result) {
            $(function () {

                Highcharts.chart('containerBucal', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['CUIDADO', 'ATENÇÃO', 'MUITO BOM'],

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
                        data: [1, 0, 2, 0]
                    }, {
                        name: 'Masculino',
                        data: [1, 0, 0, 2]
                    }]
                });

                //Highcharts.chart('containerEtnia', {
                //    chart: {
                //        type: 'bar'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    xAxis: {
                //        categories: ['PARDO', 'BRANCO', 'PRETO',
                //            'INDÍGENA', 'AMARELO'],

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    yAxis: {
                //        min: 0,
                //        title: {
                //            text: 'Total',
                //            style: {
                //                fontSize: '12px'
                //            }
                //        },

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    legend: {
                //        reversed: true,
                //        itemStyle: {
                //            fontSize: '12px'
                //        }
                //    },
                //    tooltip: {
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    plotOptions: {
                //        series: {
                //            stacking: 'normal',
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Feminino',
                //        color: '#EC407A',
                //        data: [
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                //        ]
                //    }, {
                //        name: 'Masculino',
                //        data: [

                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.resistenciaInsulina,
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoVocacionalPercentual: function (result) {
            $(function () {

                Highcharts.chart('containerVocacionalPercentual', {
                    chart: {
                        type: 'variablepie'
                    },
                    title: {
                        text: undefined
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                            '<b>{point.y} %</b> dos Alunos',
                        style: {
                            fontSize: '12px'
                        }
                    },
                    legend: {
                        itemStyle: {
                            fontSize: '2px'
                        }
                    },
                    plotOptions: {
                        variablepie: {
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
                        minPointSize: 10,
                        innerSize: '20%',
                        zMin: 0,
                        name: 'Percentual de talento vocacional dos alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'Tecnologias Aplicadas',
                            y: 5,
                            z: 50
                        }, {
                            name: 'Ciências Exatas e Naturais',
                            y: 20,
                            z: 50
                        }, {
                            name: 'Artístico',
                            y: 20,
                            z: 50
                        }, {
                            name: 'Ciências Humanas',
                            y: 30,
                            z: 50
                        }, {
                            name: 'Empreendedorismo',
                            y: 10,
                            z: 50
                        }, {
                            name: 'Ciências Contabeis e Administrativas',
                            y: 15,
                            z: 50
                        }],
                        colors: [
                            '#EF5350',
                            '#EC407A',
                            '#AB47BC',
                            '#7E57C2',
                            '#5C6BC0'
                        ]
                    }]
                });


                //Highcharts.chart('containerEtniaPercentual', {
                //    chart: {
                //        type: 'variablepie'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    tooltip: {
                //        headerFormat: '',
                //        pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' +
                //            '<b>{point.y} %</b> dos Alunos',
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    legend: {
                //        itemStyle: {
                //            fontSize: '2px'
                //        }
                //    },
                //    plotOptions: {
                //        variablepie: {
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        minPointSize: 10,
                //        innerSize: '20%',
                //        zMin: 0,
                //        name: 'Percentual de Etnia dos Alunos',
                //        borderRadius: 5,
                //        data: [{
                //            name: 'PARDO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PARDO,
                //            z: 50
                //        }, {
                //            name: 'BRANCO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.BRANCO,
                //            z: 50
                //        }, {
                //            name: 'PRETO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.PRETO,
                //            z: 50
                //        }, {
                //            name: 'INDÍGENA',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDÍGENA,
                //            z: 50
                //        }, {
                //            name: 'AMARELO',
                //            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.AMARELO,
                //            z: 50
                //        }],
                //        colors: [
                //            '#EF5350',
                //            '#EC407A',
                //            '#AB47BC',
                //            '#7E57C2'
                //        ]
                //    }]
                //});

            });
        },
        SetGraficoTotalizadorVocacional: function (result) {
            $(function () {

                Highcharts.chart('containerTalentoVocacional', {
                    chart: {
                        type: 'bar'
                    },
                    title: {
                        text: undefined
                    },
                    xAxis: {
                        categories: ['Tecnologias Aplicadas', 'Ciências Exatas e Naturais', 'Artístico', 'Ciências Humanas', 'Empreendedorismo', 'Ciências Contabeis e Administrativas']
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Total'
                        }
                    },
                    legend: {
                        reversed: true
                    },
                    plotOptions: {
                        series: {
                            stacking: 'normal',
                            dataLabels: {
                                enabled: true
                            }
                        }
                    },
                    series: [{
                        name: 'Feminino',
                        color: '#EC407A',
                        data: [1, 0, 3, 0, 1, 2]
                    }, {
                        name: 'Masculino',
                        data: [1, 1, 0, 2, 2, 4]
                    }]
                });

                //Highcharts.chart('containerEtnia', {
                //    chart: {
                //        type: 'bar'
                //    },
                //    title: {
                //        text: undefined
                //    },
                //    xAxis: {
                //        categories: ['PARDO', 'BRANCO', 'PRETO',
                //            'INDÍGENA', 'AMARELO'],

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    yAxis: {
                //        min: 0,
                //        title: {
                //            text: 'Total',
                //            style: {
                //                fontSize: '12px'
                //            }
                //        },

                //        labels: {
                //            style: {
                //                fontSize: '12px'
                //            }
                //        }
                //    },
                //    legend: {
                //        reversed: true,
                //        itemStyle: {
                //            fontSize: '12px'
                //        }
                //    },
                //    tooltip: {
                //        style: {
                //            fontSize: '12px'
                //        }
                //    },
                //    plotOptions: {
                //        series: {
                //            stacking: 'normal',
                //            dataLabels: {
                //                enabled: true,
                //                style: {
                //                    fontSize: '12px',
                //                    fontWeight: '400'
                //                }
                //            }
                //        }
                //    },
                //    series: [{
                //        name: 'Feminino',
                //        color: '#EC407A',
                //        data: [
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                //        ]
                //    }, {
                //        name: 'Masculino',
                //        data: [

                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.baixoPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.acimaPeso,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoColesterolAlto,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.riscoHipertensao,
                //            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeMasculino.resistenciaInsulina,
                //        ]
                //    }]
                //});

            });
        },
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="DashboardId"]').attr('value', id);
        $('#mdDeleteDashboard').modal('show');
        vm.DeleteDashboard(id)
    }
};
