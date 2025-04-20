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
    }
};

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
            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
            });
        }
    }
});