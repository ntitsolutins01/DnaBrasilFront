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
            self.ShowLoad(true, "pDesempenhoPerc");
            self.ShowLoad(true, "pDesempenhoTot");
            self.ShowLoad(true, "pQualidadeVidaPerc");
            self.ShowLoad(true, "pQualidadeVidaTot");
            self.ShowLoad(true, "pConsumoAlimentarPerc");
            self.ShowLoad(true, "pConsumoAlimentarTot");
            self.ShowLoad(true, "pVocacionalPerc");
            self.ShowLoad(true, "pVocacionalTot");


            const obj = {
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
                $("#laudosFemininos").text(result.data.dashboard.laudosFemininos);

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
                $('#vlProgressoConsumoAlimentar').text(result.data.dashboard.statusLaudos.progressoConsumoAlimentar + ' %');

                $("#totSaudeBucalFinalizado").text(result.data.dashboard.statusLaudos.totSaudeBucalFinalizado);
                $("#totSaudeBucalAndamento").text(result.data.dashboard.statusLaudos.totSaudeBucalAndamento);
                var percentSaudeBucal = result.data.dashboard.statusLaudos.progressoSaudeBucal + '%'
                $('#progressoSaudeBucal').css('width', percentSaudeBucal);
                $('#vlProgressoSaudeBucal').text(result.data.dashboard.statusLaudos.progressoSaudeBucal + ' %');

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

            axios.post("Dashboard/GetGraficosVocacionalByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pVocacionalPerc");
                self.ShowLoad(true, "pVocacionalTot");

                self.SetGraficoVocacionalPercentual(result);
                self.SetGraficoTotalizadorVocacional(result);

                self.ShowLoad(false, "pVocacionalPerc");
                self.ShowLoad(false, "pVocacionalTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pVocacionalPerc");
                self.ShowLoad(false, "pVocacionalTot");
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

            axios.post("Dashboard/GetGraficoPercDesempenhoFisicoMotorByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pDesempenhoPerc");
                self.ShowLoad(true, "pDesempenhoTot");

                self.SetGraficoPercDesempenhoFisicoMotor(result);
                self.SetGraficoTotDesempenhoFisicoMotor(result);

                self.ShowLoad(false, "pDesempenhoPerc");
                self.ShowLoad(false, "pDesempenhoTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pDesempenhoPerc");
                self.ShowLoad(false, "pDesempenhoTot");
            });


            axios.post("Dashboard/GetGraficosQualidadeVidaByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pQualidadeVidaPerc");
                self.ShowLoad(true, "pQualidadeVidaTot");

                self.SetGraficoQualidadePercentual(result);
                self.SetGraficoTotalizadorQualidade(result);

                self.ShowLoad(false, "pQualidadeVidaPerc");
                self.ShowLoad(false, "pQualidadeVidaTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pQualidadeVidaPerc");
                self.ShowLoad(false, "pQualidadeVidaTot");
            });


            axios.post("Dashboard/GetGraficosConsumoAlimentarByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pConsumoAlimentarPerc");
                self.ShowLoad(true, "pConsumoAlimentarTot");

                self.SetGraficoConsumoPercentual(result);
                self.SetGraficoTotalizadorConsumo(result);

                self.ShowLoad(false, "pConsumoAlimentarPerc");
                self.ShowLoad(false, "pConsumoAlimentarTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pConsumoAlimentarPerc");
                self.ShowLoad(false, "pConsumoAlimentarTot");
            });

            axios.post("Dashboard/GetGraficosSaudeBucalByFilter", obj, axiosConfig).then(result => {
                var self = this;

                self.ShowLoad(true, "pSaudeBucalPerc");
                self.ShowLoad(true, "pSaudeBucalTot");

                self.SetGraficoBuscalPercentual(result);
                self.SetGraficoTotalizadorBucal(result);

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

        },
        GetRelatorioVocacional: function () {
            var self = this;

            const obj = {
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

            axios.post("Dashboard/GetRelatorioVocacionalByFilter", obj, axiosConfig).then(result => {

                self.ShowLoad(true, "pVocacionalTot");

                $("#cadastrosMasculinos").text(result.data.dashboard.cadastrosMasculinos);
                $("#avaliacoesDna").text(result.data.dashboard.avaliacoesDna);
                $("#laudosAndamentos").text(result.data.dashboard.laudosAndamentos);
                $("#laudosFinalizados").text(result.data.dashboard.laudosFinalizados);
                $("#cadastrosFemininos").text(result.data.dashboard.cadastrosFemininos);
                $("#alunosCadastrados").text(result.data.dashboard.alunosCadastrados);
                $("#laudosMasculinos").text(result.data.dashboard.laudosMasculinos);
                $("#laudosFemininos").text(result.data.dashboard.laudosFemininos);

                self.ShowLoad(false, "pVocacionalTot");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);

                self.ShowLoad(false, "pVocacionalTot");
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
                        categories: ['Jan', 'Fev', 'Mar',
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
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaude.ABAIXODONORMAL,
                            z: 50
                        }, {
                            name: 'NORMAL',
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaude.NORMAL,
                            z: 50
                        }, {
                            name: 'SOBREPESO',
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaude.SOBREPESO,
                            z: 50
                        }, {
                            name: 'OBESIDADE',
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaude.OBESIDADE,
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
                        categories: ['Risco de Colesterol Alto e Diabetes',
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
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoColesterolAlto,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.riscoHipertensao,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.resistenciaInsulina,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.desequilibrioMuscular,
                            result.data.dashboard.listTotalizadorSaudeSexo.valorTotalizadorSaudeFeminino.indicePositivoSaude
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
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
                        name: 'Percentual de Desempenho Fisico Motor dos Alunos',
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
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.shutlleRun,
                            z: 50
                        }, {
                            name: 'Força do Core',
                            y: result.data.dashboard.listTotalizadorDesempenho.percDesempenho.prancha,
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
                        categories: ['Velocidade', 'Flexibilidade Muscular', 'Força de Membros Superiores', 'Força Explosiva de Membros Inferiores', 'Aptidão Cardiorrespiratória', 'Agilidade', 'Força do Core'],

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
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.shutlleRun,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoFeminino.prancha
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.velocidade,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.flexibilidadeMuscular,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.forcaMembrosSup,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.forcaExplosiva,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.aptidaoCardio,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.shutlleRun,
                            result.data.dashboard.listTotalizadorDesempenho.valorTotalizadorDesempenhoMasculino.prancha
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
                            name: 'INDIGENA',
                            y: result.data.dashboard.listTotalizadorEtnia.percTotalizadorEtniaMasculino.INDIGENA,
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
                            'INDIGENA', 'AMARELO'],

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
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.INDIGENA,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaFeminino.AMARELO,
                        ]
                    }, {
                        name: 'Masculino',
                        data: [

                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.PARDO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.BRANCO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.PRETO,
                            result.data.dashboard.listTotalizadorEtnia.valorTotalizadorEtniaMasculino.INDIGENA,
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
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.BemEstarFisico,
                            z: 50
                        }, {
                            name: 'Mal estar físico',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.MalEstarFisico,
                            z: 50
                        }, {
                            name: 'Autoestima e estabilidade emocional',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.AutoEstima,
                            z: 50
                        }, {
                            name: 'Baixa autoestima e dificuldades emocionais',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.BaixaAutoEstima,
                            z: 50
                        }, {
                            name: 'Funcionamento harmônico familiar',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.FuncionamentoHarmonico,
                            z: 50
                        }, {
                            name: 'Conflitos no contexto familiar',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.Conflitos,
                            z: 50
                        }, {
                            name: 'Contextos favorecedores do desenvolvimento ',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.ContextosFavorecedores,
                            z: 50
                        }, {
                            name: 'Contextos não favorecedores do desenvolvimento ',
                            y: result.data.dashboard.listTotalizadorQualidadeVida.percentualQualidade.ContextosNaoFavorecedores,
                            z: 50
                        }]
                    }]
                });
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
                            'Conflitos no contexto familiar', 'Contextos favorecedores do desenvolvimento ', 'Contextos não favorecedores do desenvolvimento '],

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
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.BemEstarFisico,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.MalEstarFisico,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.AutoEstima,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.BaixaAutoEstima, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.FuncionamentoHarmonico, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.Conflitos, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.ContextosFavorecedores, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaFeminino.ContextosNaoFavorecedores
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.BemEstarFisico,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.MalEstarFisico,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.AutoEstima,
                            result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.BaixaAutoEstima, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.FuncionamentoHarmonico, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.Conflitos, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.ContextosFavorecedores, result.data.dashboard.listTotalizadorQualidadeVida.valorTotalizadorQualidadeVidaMasculino.ContextosNaoFavorecedores
                        ]
                    }]
                });
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
                        name: 'Percentual de hábitos alimentares dos alunos',
                        borderRadius: 5,
                        data: [{
                            name: 'Hábitos não saudáveis',
                            y: result.data.dashboard.listTotalizadorConsumoAlimentar.percentualConsumoAlimentar.HabitosSaudaveis,
                            z: 50
                        }, {
                            name: 'Hábitos satisfatórios',
                            y: result.data.dashboard.listTotalizadorConsumoAlimentar.percentualConsumoAlimentar.HabitosNaoSaudaveis,
                            z: 50
                        }, {
                            name: 'Bons Hábitos alimentares',
                            y: result.data.dashboard.listTotalizadorConsumoAlimentar.percentualConsumoAlimentar.HabitosSatisfatorios,
                            z: 50
                        }, {
                            name: 'Hábitos Saudáveis',
                            y: result.data.dashboard.listTotalizadorConsumoAlimentar.percentualConsumoAlimentar.BonsHabitosAlimentares,
                            z: 50
                        }]
                    }]
                });
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
                            result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarFeminino.HabitosSaudaveis, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarFeminino.HabitosNaoSaudaveis, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarFeminino.HabitosSatisfatorios, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarFeminino.BonsHabitosAlimentares
                        ]
                    }, {
                        name: 'Masculino',
                        data: [result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarMasculino.HabitosSaudaveis, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarMasculino.HabitosNaoSaudaveis, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarMasculino.HabitosSatisfatorios, result.data.dashboard.listTotalizadorConsumoAlimentar.valorTotalizadorConsumoAlimentarMasculino.BonsHabitosAlimentares]
                    }]
                });

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
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaudeBucal.CUIDADO,
                            z: 50
                        }, {
                            name: 'ATENÇÃO',
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaudeBucal.ATENCAO,
                            z: 50
                        }, {
                            name: 'MUITO BOM',
                            y: result.data.dashboard.listTotalizadorSaudeBucal.percentualSaudeBucal.MUITOBOM,
                            z: 50
                        }]
                    }]
                });
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
                        data: [result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalFeminino.CUIDADO, result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalFeminino.ATENCAO, result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalFeminino.MUITOBOM]
                    }, {
                        name: 'Masculino',
                        data: [result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalMasculino.CUIDADO, result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalMasculino.ATENCAO, result.data.dashboard.listTotalizadorSaudeBucal.valorTotalizadorSaudeBucalMasculino.MUITOBOM]
                    }]
                });
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
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.TecnologiasAplicadas,
                            z: 50
                        }, {
                            name: 'Ciências Exatas e Naturais',
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.CienciasExatasNaturais,
                            z: 50
                        }, {
                            name: 'Artístico',
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.Artistico,
                            z: 50
                        }, {
                            name: 'Ciências Humanas',
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.CienciasHumanas,
                            z: 50
                        }, {
                            name: 'Empreendedorismo',
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.Empreendedorismo,
                            z: 50
                        }, {
                            name: 'Ciências Contabeis e Administrativas',
                            y: result.data.dashboard.listTotalizadorVocacional.percentualVocacional.CienciasContabeisAdministrativas,
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
                        categories: ['Tecnologias Aplicadas', 'Ciências Exatas e Naturais', 'Artístico', 'Ciências Humanas', 'Empreendedorismo', 'Ciências Contabeis e Administrativas'],

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
                            result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.TecnologiasAplicadas, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.CienciasExatasNaturais, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.Artistico, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.CienciasHumanas, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.Empreendedorismo, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalFeminino.CienciasContabeisAdministrativas
                        ]
                    }, {
                        name: 'Masculino',
                        data: [
                            result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.TecnologiasAplicadas, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.CienciasExatasNaturais, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.Artistico, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.CienciasHumanas, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.Empreendedorismo, result.data.dashboard.listTotalizadorVocacional.valorTotalizadorVocacionalMasculino.CienciasContabeisAdministrativas
                        ]
                    }]
                });
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
