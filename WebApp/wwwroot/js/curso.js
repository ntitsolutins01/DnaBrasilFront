var vm = new Vue({
    el: "#vCurso ",
    data: {
        loading: false,
        editDto: { Id: "", Titulo: "", Descricao: "", CargaHoraria: "", Status: true, Imagem: "", NomeImagem:"" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            //mascara dos inputs
            var cargaHoraria = $("#cargaHoraria");
            cargaHoraria.mask('000', { reverse: false });

            //skin select2 combo
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

            var formid = $('form')[1].id;

            if (formid === "formEditCurso") {

                $("#formEditCurso ").validate({
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

            if (formid === "formCurso") {


                $("#formCurso").validate({
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
        DeleteCurso: function (id) {
            var url = "Curso/Delete/" + id;
            $("#deleteCursoHref").prop("href", url);
        },
        EditCurso: function (id) {
            var self = this;

            self.editDto = { Id: "", Titulo: "", Descricao: "", CargaHoraria: "", Status: true, Imagem: "", NomeImagem: "" };

            axios.get("Curso/GetCursoById/?id=" + id).then(result => {

                self.$nextTick(() => {
                    self.editDto = {
                        Id: result.data.id,
                        Titulo: result.data.titulo,
                        Descricao: result.data.descricao,
                        CargaHoraria: result.data.cargaHoraria,
                        Status: result.data.status,
                        Imagem: "\Cursos" + result.data.imagem.split("\Cursos")[1],
                        NomeImagem: result.data.nomeImagem
                    };
                });

                self.$nextTick(() => {
                    $("#descricao").val(result.data.descricao || '');

                    $("#descricao")[0].dispatchEvent(new Event('input'));
                });

                if (result.data.listCoordenadores.length > 0) {
                    var items = '<option value="">Selecionar o Coordenador</option>';
                    $("#ddlCoordenador").empty;
                    $.each(result.data.listCoordenadores,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlCoordenador").html(items);
                }
                else {
                    new PNotify({
                        title: 'Coordenador',
                        text: 'Coordenadores não encontrados.',
                        type: 'warning'
                    });
                }

            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteCursoId"]').attr('value', id);
        $('#mdDeleteCurso').modal('show');
        vm.DeleteCurso(id)
    },
    EditModal: function (id) {
        $('input[name="editCursoId"]').attr('value', id);
        $('#mdEditCurso').modal('show');
        vm.EditCurso(id)
    }
};