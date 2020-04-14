// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: game.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_game_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_game_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3011000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3011004 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_table_driven.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/inlined_string_field.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/generated_enum_reflection.h>
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_game_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_game_2eproto {
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTableField entries[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::AuxillaryParseTableField aux[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTable schema[3]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::FieldMetadata field_metadata[];
  static const ::PROTOBUF_NAMESPACE_ID::internal::SerializationTable serialization_table[];
  static const ::PROTOBUF_NAMESPACE_ID::uint32 offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_game_2eproto;
class GuestLoginReq;
class GuestLoginReqDefaultTypeInternal;
extern GuestLoginReqDefaultTypeInternal _GuestLoginReq_default_instance_;
class GuestLoginRes;
class GuestLoginResDefaultTypeInternal;
extern GuestLoginResDefaultTypeInternal _GuestLoginRes_default_instance_;
class UserCenterInfo;
class UserCenterInfoDefaultTypeInternal;
extern UserCenterInfoDefaultTypeInternal _UserCenterInfo_default_instance_;
PROTOBUF_NAMESPACE_OPEN
template<> ::GuestLoginReq* Arena::CreateMaybeMessage<::GuestLoginReq>(Arena*);
template<> ::GuestLoginRes* Arena::CreateMaybeMessage<::GuestLoginRes>(Arena*);
template<> ::UserCenterInfo* Arena::CreateMaybeMessage<::UserCenterInfo>(Arena*);
PROTOBUF_NAMESPACE_CLOSE

enum Stype : int {
  INVALID_STYPE = 0,
  Auth = 1,
  System = 2,
  Logic = 3
};
bool Stype_IsValid(int value);
constexpr Stype Stype_MIN = INVALID_STYPE;
constexpr Stype Stype_MAX = Logic;
constexpr int Stype_ARRAYSIZE = Stype_MAX + 1;

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* Stype_descriptor();
template<typename T>
inline const std::string& Stype_Name(T enum_t_value) {
  static_assert(::std::is_same<T, Stype>::value ||
    ::std::is_integral<T>::value,
    "Incorrect type passed to function Stype_Name.");
  return ::PROTOBUF_NAMESPACE_ID::internal::NameOfEnum(
    Stype_descriptor(), enum_t_value);
}
inline bool Stype_Parse(
    const std::string& name, Stype* value) {
  return ::PROTOBUF_NAMESPACE_ID::internal::ParseNamedEnum<Stype>(
    Stype_descriptor(), name, value);
}
enum Cmd : int {
  INVALID_CMD = 0,
  eGuestLoginReq = 1,
  eGuestLoginRes = 2
};
bool Cmd_IsValid(int value);
constexpr Cmd Cmd_MIN = INVALID_CMD;
constexpr Cmd Cmd_MAX = eGuestLoginRes;
constexpr int Cmd_ARRAYSIZE = Cmd_MAX + 1;

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* Cmd_descriptor();
template<typename T>
inline const std::string& Cmd_Name(T enum_t_value) {
  static_assert(::std::is_same<T, Cmd>::value ||
    ::std::is_integral<T>::value,
    "Incorrect type passed to function Cmd_Name.");
  return ::PROTOBUF_NAMESPACE_ID::internal::NameOfEnum(
    Cmd_descriptor(), enum_t_value);
}
inline bool Cmd_Parse(
    const std::string& name, Cmd* value) {
  return ::PROTOBUF_NAMESPACE_ID::internal::ParseNamedEnum<Cmd>(
    Cmd_descriptor(), name, value);
}
// ===================================================================

class GuestLoginReq :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:GuestLoginReq) */ {
 public:
  GuestLoginReq();
  virtual ~GuestLoginReq();

  GuestLoginReq(const GuestLoginReq& from);
  GuestLoginReq(GuestLoginReq&& from) noexcept
    : GuestLoginReq() {
    *this = ::std::move(from);
  }

  inline GuestLoginReq& operator=(const GuestLoginReq& from) {
    CopyFrom(from);
    return *this;
  }
  inline GuestLoginReq& operator=(GuestLoginReq&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  inline const ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet& unknown_fields() const {
    return _internal_metadata_.unknown_fields();
  }
  inline ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet* mutable_unknown_fields() {
    return _internal_metadata_.mutable_unknown_fields();
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const GuestLoginReq& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const GuestLoginReq* internal_default_instance() {
    return reinterpret_cast<const GuestLoginReq*>(
               &_GuestLoginReq_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  friend void swap(GuestLoginReq& a, GuestLoginReq& b) {
    a.Swap(&b);
  }
  inline void Swap(GuestLoginReq* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline GuestLoginReq* New() const final {
    return CreateMaybeMessage<GuestLoginReq>(nullptr);
  }

  GuestLoginReq* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<GuestLoginReq>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const GuestLoginReq& from);
  void MergeFrom(const GuestLoginReq& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(GuestLoginReq* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "GuestLoginReq";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_game_2eproto);
    return ::descriptor_table_game_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kGuestKeyFieldNumber = 1,
  };
  // required string guest_key = 1;
  bool has_guest_key() const;
  private:
  bool _internal_has_guest_key() const;
  public:
  void clear_guest_key();
  const std::string& guest_key() const;
  void set_guest_key(const std::string& value);
  void set_guest_key(std::string&& value);
  void set_guest_key(const char* value);
  void set_guest_key(const char* value, size_t size);
  std::string* mutable_guest_key();
  std::string* release_guest_key();
  void set_allocated_guest_key(std::string* guest_key);
  private:
  const std::string& _internal_guest_key() const;
  void _internal_set_guest_key(const std::string& value);
  std::string* _internal_mutable_guest_key();
  public:

  // @@protoc_insertion_point(class_scope:GuestLoginReq)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::HasBits<1> _has_bits_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr guest_key_;
  friend struct ::TableStruct_game_2eproto;
};
// -------------------------------------------------------------------

class UserCenterInfo :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:UserCenterInfo) */ {
 public:
  UserCenterInfo();
  virtual ~UserCenterInfo();

  UserCenterInfo(const UserCenterInfo& from);
  UserCenterInfo(UserCenterInfo&& from) noexcept
    : UserCenterInfo() {
    *this = ::std::move(from);
  }

  inline UserCenterInfo& operator=(const UserCenterInfo& from) {
    CopyFrom(from);
    return *this;
  }
  inline UserCenterInfo& operator=(UserCenterInfo&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  inline const ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet& unknown_fields() const {
    return _internal_metadata_.unknown_fields();
  }
  inline ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet* mutable_unknown_fields() {
    return _internal_metadata_.mutable_unknown_fields();
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const UserCenterInfo& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const UserCenterInfo* internal_default_instance() {
    return reinterpret_cast<const UserCenterInfo*>(
               &_UserCenterInfo_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    1;

  friend void swap(UserCenterInfo& a, UserCenterInfo& b) {
    a.Swap(&b);
  }
  inline void Swap(UserCenterInfo* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline UserCenterInfo* New() const final {
    return CreateMaybeMessage<UserCenterInfo>(nullptr);
  }

  UserCenterInfo* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<UserCenterInfo>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const UserCenterInfo& from);
  void MergeFrom(const UserCenterInfo& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(UserCenterInfo* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "UserCenterInfo";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_game_2eproto);
    return ::descriptor_table_game_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kUnickFieldNumber = 1,
    kUfaceFieldNumber = 2,
    kUsexFieldNumber = 3,
    kUvipFieldNumber = 4,
    kUidFieldNumber = 5,
  };
  // required string unick = 1;
  bool has_unick() const;
  private:
  bool _internal_has_unick() const;
  public:
  void clear_unick();
  const std::string& unick() const;
  void set_unick(const std::string& value);
  void set_unick(std::string&& value);
  void set_unick(const char* value);
  void set_unick(const char* value, size_t size);
  std::string* mutable_unick();
  std::string* release_unick();
  void set_allocated_unick(std::string* unick);
  private:
  const std::string& _internal_unick() const;
  void _internal_set_unick(const std::string& value);
  std::string* _internal_mutable_unick();
  public:

  // required int32 uface = 2;
  bool has_uface() const;
  private:
  bool _internal_has_uface() const;
  public:
  void clear_uface();
  ::PROTOBUF_NAMESPACE_ID::int32 uface() const;
  void set_uface(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_uface() const;
  void _internal_set_uface(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // required int32 usex = 3;
  bool has_usex() const;
  private:
  bool _internal_has_usex() const;
  public:
  void clear_usex();
  ::PROTOBUF_NAMESPACE_ID::int32 usex() const;
  void set_usex(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_usex() const;
  void _internal_set_usex(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // required int32 uvip = 4;
  bool has_uvip() const;
  private:
  bool _internal_has_uvip() const;
  public:
  void clear_uvip();
  ::PROTOBUF_NAMESPACE_ID::int32 uvip() const;
  void set_uvip(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_uvip() const;
  void _internal_set_uvip(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // required int32 uid = 5;
  bool has_uid() const;
  private:
  bool _internal_has_uid() const;
  public:
  void clear_uid();
  ::PROTOBUF_NAMESPACE_ID::int32 uid() const;
  void set_uid(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_uid() const;
  void _internal_set_uid(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // @@protoc_insertion_point(class_scope:UserCenterInfo)
 private:
  class _Internal;

  // helper for ByteSizeLong()
  size_t RequiredFieldsByteSizeFallback() const;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::HasBits<1> _has_bits_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr unick_;
  ::PROTOBUF_NAMESPACE_ID::int32 uface_;
  ::PROTOBUF_NAMESPACE_ID::int32 usex_;
  ::PROTOBUF_NAMESPACE_ID::int32 uvip_;
  ::PROTOBUF_NAMESPACE_ID::int32 uid_;
  friend struct ::TableStruct_game_2eproto;
};
// -------------------------------------------------------------------

class GuestLoginRes :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:GuestLoginRes) */ {
 public:
  GuestLoginRes();
  virtual ~GuestLoginRes();

  GuestLoginRes(const GuestLoginRes& from);
  GuestLoginRes(GuestLoginRes&& from) noexcept
    : GuestLoginRes() {
    *this = ::std::move(from);
  }

  inline GuestLoginRes& operator=(const GuestLoginRes& from) {
    CopyFrom(from);
    return *this;
  }
  inline GuestLoginRes& operator=(GuestLoginRes&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  inline const ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet& unknown_fields() const {
    return _internal_metadata_.unknown_fields();
  }
  inline ::PROTOBUF_NAMESPACE_ID::UnknownFieldSet* mutable_unknown_fields() {
    return _internal_metadata_.mutable_unknown_fields();
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const GuestLoginRes& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const GuestLoginRes* internal_default_instance() {
    return reinterpret_cast<const GuestLoginRes*>(
               &_GuestLoginRes_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    2;

  friend void swap(GuestLoginRes& a, GuestLoginRes& b) {
    a.Swap(&b);
  }
  inline void Swap(GuestLoginRes* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline GuestLoginRes* New() const final {
    return CreateMaybeMessage<GuestLoginRes>(nullptr);
  }

  GuestLoginRes* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<GuestLoginRes>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const GuestLoginRes& from);
  void MergeFrom(const GuestLoginRes& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(GuestLoginRes* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "GuestLoginRes";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_game_2eproto);
    return ::descriptor_table_game_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kUinfoFieldNumber = 2,
    kStatusFieldNumber = 1,
  };
  // optional .UserCenterInfo uinfo = 2;
  bool has_uinfo() const;
  private:
  bool _internal_has_uinfo() const;
  public:
  void clear_uinfo();
  const ::UserCenterInfo& uinfo() const;
  ::UserCenterInfo* release_uinfo();
  ::UserCenterInfo* mutable_uinfo();
  void set_allocated_uinfo(::UserCenterInfo* uinfo);
  private:
  const ::UserCenterInfo& _internal_uinfo() const;
  ::UserCenterInfo* _internal_mutable_uinfo();
  public:

  // required int32 status = 1;
  bool has_status() const;
  private:
  bool _internal_has_status() const;
  public:
  void clear_status();
  ::PROTOBUF_NAMESPACE_ID::int32 status() const;
  void set_status(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_status() const;
  void _internal_set_status(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // @@protoc_insertion_point(class_scope:GuestLoginRes)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::HasBits<1> _has_bits_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  ::UserCenterInfo* uinfo_;
  ::PROTOBUF_NAMESPACE_ID::int32 status_;
  friend struct ::TableStruct_game_2eproto;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// GuestLoginReq

// required string guest_key = 1;
inline bool GuestLoginReq::_internal_has_guest_key() const {
  bool value = (_has_bits_[0] & 0x00000001u) != 0;
  return value;
}
inline bool GuestLoginReq::has_guest_key() const {
  return _internal_has_guest_key();
}
inline void GuestLoginReq::clear_guest_key() {
  guest_key_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  _has_bits_[0] &= ~0x00000001u;
}
inline const std::string& GuestLoginReq::guest_key() const {
  // @@protoc_insertion_point(field_get:GuestLoginReq.guest_key)
  return _internal_guest_key();
}
inline void GuestLoginReq::set_guest_key(const std::string& value) {
  _internal_set_guest_key(value);
  // @@protoc_insertion_point(field_set:GuestLoginReq.guest_key)
}
inline std::string* GuestLoginReq::mutable_guest_key() {
  // @@protoc_insertion_point(field_mutable:GuestLoginReq.guest_key)
  return _internal_mutable_guest_key();
}
inline const std::string& GuestLoginReq::_internal_guest_key() const {
  return guest_key_.GetNoArena();
}
inline void GuestLoginReq::_internal_set_guest_key(const std::string& value) {
  _has_bits_[0] |= 0x00000001u;
  guest_key_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void GuestLoginReq::set_guest_key(std::string&& value) {
  _has_bits_[0] |= 0x00000001u;
  guest_key_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:GuestLoginReq.guest_key)
}
inline void GuestLoginReq::set_guest_key(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  _has_bits_[0] |= 0x00000001u;
  guest_key_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:GuestLoginReq.guest_key)
}
inline void GuestLoginReq::set_guest_key(const char* value, size_t size) {
  _has_bits_[0] |= 0x00000001u;
  guest_key_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:GuestLoginReq.guest_key)
}
inline std::string* GuestLoginReq::_internal_mutable_guest_key() {
  _has_bits_[0] |= 0x00000001u;
  return guest_key_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* GuestLoginReq::release_guest_key() {
  // @@protoc_insertion_point(field_release:GuestLoginReq.guest_key)
  if (!_internal_has_guest_key()) {
    return nullptr;
  }
  _has_bits_[0] &= ~0x00000001u;
  return guest_key_.ReleaseNonDefaultNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void GuestLoginReq::set_allocated_guest_key(std::string* guest_key) {
  if (guest_key != nullptr) {
    _has_bits_[0] |= 0x00000001u;
  } else {
    _has_bits_[0] &= ~0x00000001u;
  }
  guest_key_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), guest_key);
  // @@protoc_insertion_point(field_set_allocated:GuestLoginReq.guest_key)
}

// -------------------------------------------------------------------

// UserCenterInfo

// required string unick = 1;
inline bool UserCenterInfo::_internal_has_unick() const {
  bool value = (_has_bits_[0] & 0x00000001u) != 0;
  return value;
}
inline bool UserCenterInfo::has_unick() const {
  return _internal_has_unick();
}
inline void UserCenterInfo::clear_unick() {
  unick_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  _has_bits_[0] &= ~0x00000001u;
}
inline const std::string& UserCenterInfo::unick() const {
  // @@protoc_insertion_point(field_get:UserCenterInfo.unick)
  return _internal_unick();
}
inline void UserCenterInfo::set_unick(const std::string& value) {
  _internal_set_unick(value);
  // @@protoc_insertion_point(field_set:UserCenterInfo.unick)
}
inline std::string* UserCenterInfo::mutable_unick() {
  // @@protoc_insertion_point(field_mutable:UserCenterInfo.unick)
  return _internal_mutable_unick();
}
inline const std::string& UserCenterInfo::_internal_unick() const {
  return unick_.GetNoArena();
}
inline void UserCenterInfo::_internal_set_unick(const std::string& value) {
  _has_bits_[0] |= 0x00000001u;
  unick_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void UserCenterInfo::set_unick(std::string&& value) {
  _has_bits_[0] |= 0x00000001u;
  unick_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:UserCenterInfo.unick)
}
inline void UserCenterInfo::set_unick(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  _has_bits_[0] |= 0x00000001u;
  unick_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:UserCenterInfo.unick)
}
inline void UserCenterInfo::set_unick(const char* value, size_t size) {
  _has_bits_[0] |= 0x00000001u;
  unick_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:UserCenterInfo.unick)
}
inline std::string* UserCenterInfo::_internal_mutable_unick() {
  _has_bits_[0] |= 0x00000001u;
  return unick_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* UserCenterInfo::release_unick() {
  // @@protoc_insertion_point(field_release:UserCenterInfo.unick)
  if (!_internal_has_unick()) {
    return nullptr;
  }
  _has_bits_[0] &= ~0x00000001u;
  return unick_.ReleaseNonDefaultNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void UserCenterInfo::set_allocated_unick(std::string* unick) {
  if (unick != nullptr) {
    _has_bits_[0] |= 0x00000001u;
  } else {
    _has_bits_[0] &= ~0x00000001u;
  }
  unick_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), unick);
  // @@protoc_insertion_point(field_set_allocated:UserCenterInfo.unick)
}

// required int32 uface = 2;
inline bool UserCenterInfo::_internal_has_uface() const {
  bool value = (_has_bits_[0] & 0x00000002u) != 0;
  return value;
}
inline bool UserCenterInfo::has_uface() const {
  return _internal_has_uface();
}
inline void UserCenterInfo::clear_uface() {
  uface_ = 0;
  _has_bits_[0] &= ~0x00000002u;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::_internal_uface() const {
  return uface_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::uface() const {
  // @@protoc_insertion_point(field_get:UserCenterInfo.uface)
  return _internal_uface();
}
inline void UserCenterInfo::_internal_set_uface(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _has_bits_[0] |= 0x00000002u;
  uface_ = value;
}
inline void UserCenterInfo::set_uface(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_uface(value);
  // @@protoc_insertion_point(field_set:UserCenterInfo.uface)
}

// required int32 usex = 3;
inline bool UserCenterInfo::_internal_has_usex() const {
  bool value = (_has_bits_[0] & 0x00000004u) != 0;
  return value;
}
inline bool UserCenterInfo::has_usex() const {
  return _internal_has_usex();
}
inline void UserCenterInfo::clear_usex() {
  usex_ = 0;
  _has_bits_[0] &= ~0x00000004u;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::_internal_usex() const {
  return usex_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::usex() const {
  // @@protoc_insertion_point(field_get:UserCenterInfo.usex)
  return _internal_usex();
}
inline void UserCenterInfo::_internal_set_usex(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _has_bits_[0] |= 0x00000004u;
  usex_ = value;
}
inline void UserCenterInfo::set_usex(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_usex(value);
  // @@protoc_insertion_point(field_set:UserCenterInfo.usex)
}

// required int32 uvip = 4;
inline bool UserCenterInfo::_internal_has_uvip() const {
  bool value = (_has_bits_[0] & 0x00000008u) != 0;
  return value;
}
inline bool UserCenterInfo::has_uvip() const {
  return _internal_has_uvip();
}
inline void UserCenterInfo::clear_uvip() {
  uvip_ = 0;
  _has_bits_[0] &= ~0x00000008u;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::_internal_uvip() const {
  return uvip_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::uvip() const {
  // @@protoc_insertion_point(field_get:UserCenterInfo.uvip)
  return _internal_uvip();
}
inline void UserCenterInfo::_internal_set_uvip(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _has_bits_[0] |= 0x00000008u;
  uvip_ = value;
}
inline void UserCenterInfo::set_uvip(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_uvip(value);
  // @@protoc_insertion_point(field_set:UserCenterInfo.uvip)
}

// required int32 uid = 5;
inline bool UserCenterInfo::_internal_has_uid() const {
  bool value = (_has_bits_[0] & 0x00000010u) != 0;
  return value;
}
inline bool UserCenterInfo::has_uid() const {
  return _internal_has_uid();
}
inline void UserCenterInfo::clear_uid() {
  uid_ = 0;
  _has_bits_[0] &= ~0x00000010u;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::_internal_uid() const {
  return uid_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 UserCenterInfo::uid() const {
  // @@protoc_insertion_point(field_get:UserCenterInfo.uid)
  return _internal_uid();
}
inline void UserCenterInfo::_internal_set_uid(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _has_bits_[0] |= 0x00000010u;
  uid_ = value;
}
inline void UserCenterInfo::set_uid(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_uid(value);
  // @@protoc_insertion_point(field_set:UserCenterInfo.uid)
}

// -------------------------------------------------------------------

// GuestLoginRes

// required int32 status = 1;
inline bool GuestLoginRes::_internal_has_status() const {
  bool value = (_has_bits_[0] & 0x00000002u) != 0;
  return value;
}
inline bool GuestLoginRes::has_status() const {
  return _internal_has_status();
}
inline void GuestLoginRes::clear_status() {
  status_ = 0;
  _has_bits_[0] &= ~0x00000002u;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 GuestLoginRes::_internal_status() const {
  return status_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 GuestLoginRes::status() const {
  // @@protoc_insertion_point(field_get:GuestLoginRes.status)
  return _internal_status();
}
inline void GuestLoginRes::_internal_set_status(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _has_bits_[0] |= 0x00000002u;
  status_ = value;
}
inline void GuestLoginRes::set_status(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_status(value);
  // @@protoc_insertion_point(field_set:GuestLoginRes.status)
}

// optional .UserCenterInfo uinfo = 2;
inline bool GuestLoginRes::_internal_has_uinfo() const {
  bool value = (_has_bits_[0] & 0x00000001u) != 0;
  PROTOBUF_ASSUME(!value || uinfo_ != nullptr);
  return value;
}
inline bool GuestLoginRes::has_uinfo() const {
  return _internal_has_uinfo();
}
inline void GuestLoginRes::clear_uinfo() {
  if (uinfo_ != nullptr) uinfo_->Clear();
  _has_bits_[0] &= ~0x00000001u;
}
inline const ::UserCenterInfo& GuestLoginRes::_internal_uinfo() const {
  const ::UserCenterInfo* p = uinfo_;
  return p != nullptr ? *p : *reinterpret_cast<const ::UserCenterInfo*>(
      &::_UserCenterInfo_default_instance_);
}
inline const ::UserCenterInfo& GuestLoginRes::uinfo() const {
  // @@protoc_insertion_point(field_get:GuestLoginRes.uinfo)
  return _internal_uinfo();
}
inline ::UserCenterInfo* GuestLoginRes::release_uinfo() {
  // @@protoc_insertion_point(field_release:GuestLoginRes.uinfo)
  _has_bits_[0] &= ~0x00000001u;
  ::UserCenterInfo* temp = uinfo_;
  uinfo_ = nullptr;
  return temp;
}
inline ::UserCenterInfo* GuestLoginRes::_internal_mutable_uinfo() {
  _has_bits_[0] |= 0x00000001u;
  if (uinfo_ == nullptr) {
    auto* p = CreateMaybeMessage<::UserCenterInfo>(GetArenaNoVirtual());
    uinfo_ = p;
  }
  return uinfo_;
}
inline ::UserCenterInfo* GuestLoginRes::mutable_uinfo() {
  // @@protoc_insertion_point(field_mutable:GuestLoginRes.uinfo)
  return _internal_mutable_uinfo();
}
inline void GuestLoginRes::set_allocated_uinfo(::UserCenterInfo* uinfo) {
  ::PROTOBUF_NAMESPACE_ID::Arena* message_arena = GetArenaNoVirtual();
  if (message_arena == nullptr) {
    delete uinfo_;
  }
  if (uinfo) {
    ::PROTOBUF_NAMESPACE_ID::Arena* submessage_arena = nullptr;
    if (message_arena != submessage_arena) {
      uinfo = ::PROTOBUF_NAMESPACE_ID::internal::GetOwnedMessage(
          message_arena, uinfo, submessage_arena);
    }
    _has_bits_[0] |= 0x00000001u;
  } else {
    _has_bits_[0] &= ~0x00000001u;
  }
  uinfo_ = uinfo;
  // @@protoc_insertion_point(field_set_allocated:GuestLoginRes.uinfo)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__
// -------------------------------------------------------------------

// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)


PROTOBUF_NAMESPACE_OPEN

template <> struct is_proto_enum< ::Stype> : ::std::true_type {};
template <>
inline const EnumDescriptor* GetEnumDescriptor< ::Stype>() {
  return ::Stype_descriptor();
}
template <> struct is_proto_enum< ::Cmd> : ::std::true_type {};
template <>
inline const EnumDescriptor* GetEnumDescriptor< ::Cmd>() {
  return ::Cmd_descriptor();
}

PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_game_2eproto