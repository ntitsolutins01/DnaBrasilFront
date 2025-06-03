var vm = new Vue({
    el: "#vCurso",
    data: {
        loading: false,
        editDto: {
            Id: "",
            Titulo: "",
            Descricao: "",
            CargaHoraria: "",
            Status: true,
            Imagem: "",
            NomeImagem: ""
        },
        novaOrdem: ""
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // Inicialização de componentes
            var cargaHoraria = $("#cargaHoraria");
            cargaHoraria.mask('000', { reverse: false });

            // Inicialização do Select2
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

            // Inicialização do Switch
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {
                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);
                        $this.themePluginIOS7Switch();
                    });
                });
            }

        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            this.loading = flag;
            $("#" + el).loadingOverlay({ "startShowing": flag });
            if (!flag) {
                $("#" + el).removeClass("loading-overlay-showing");
            } else {
                $("#" + el).addClass("loading-overlay-showing");
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
                        Imagem: result.data.imagem && result.data.imagem.includes("\\Cursos")
                            ? "\\Cursos" + result.data.imagem.split("\\Cursos")[1]
                            : null,
                        NomeImagem: result.data.nomeImagem
                    };
                });

                if (result.data.listCoordenadores && result.data.listCoordenadores.length > 0) {
                    var items = '<option value="">Selecionar o Coordenador</option>';
                    $("#ddlCoordenador").empty();
                    $.each(result.data.listCoordenadores,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlCoordenador").html(items);
                } else {
                    Site.Notification("Coordenador", "Coordenadores não encontrados.", "warning", 1);
                }
            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
            });
        },
        ValidateFileType: function() {
            var fileName = document.getElementById("arquivo").value;
            var idxDot = fileName.lastIndexOf(".") + 1;
            var extFile = fileName.substr(idxDot, fileName.length).toLowerCase();
            if (extFile === "jpg" || extFile === "jpeg" || extFile === "png") {
                //TO DO
            } else {
                Site.Notification("Erro ao realizar Upload", "Somente arquivos jpg/jpeg e png são permitidos.", "error", 2);
                
            }   
        }

    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteCursoId"]').val(id);
        $('#mdDeleteCurso').modal('show');
        vm.DeleteCurso(id);
    },
    EditModal: function (id) {
        $('input[name="editCursoId"]').val(id);
        $('#mdEditCurso').modal('show');
        vm.EditCurso(id);
    },

};
